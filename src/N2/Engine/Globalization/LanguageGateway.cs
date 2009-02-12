using System;
using System.Collections.Generic;
using N2.Persistence;
using N2.Web;
using N2.Collections;
using N2.Persistence.Finder;
using N2.Edit;
using N2.Definitions;
using N2.Security;
using N2.Configuration;
using N2.Edit.Trash;

namespace N2.Engine.Globalization
{
    /// <summary>
    /// Globalization handler.
    /// </summary>
	public class LanguageGateway : ILanguageGateway
	{
		public const string LanguageKey = "LanguageKey";

		readonly IPersister persister;
		readonly IItemFinder finder;
		readonly IEditManager editManager;
		readonly IDefinitionManager definitions;
		readonly IHost host;
		int recursionDepth = 3;
        ISecurityManager security;
        IWebContext context;
        bool enabled = true;

        public LanguageGateway(IPersister persister, IItemFinder finder, IEditManager editManager, IDefinitionManager definitions, IHost host, ISecurityManager security, IWebContext context, EngineSection config)
            : this(persister, finder, editManager, definitions, host, security, context)
        {
            Enabled = config.Globalization.Enabled;
        }

		public LanguageGateway(IPersister persister, IItemFinder finder, IEditManager editManager, IDefinitionManager definitions, IHost host, ISecurityManager security, IWebContext context)
		{
			this.persister = persister;
			this.finder = finder;
			this.editManager = editManager;
			this.definitions = definitions;
			this.host = host;
            this.security = security;
            this.context = context;
        }

        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        public int RecursionDepth
        {
            get { return recursionDepth; }
            set { recursionDepth = value; }
        }

		public ILanguage GetLanguage(ContentItem item)
		{
			foreach (ContentItem ancestor in Find.EnumerateParents(item, null, true))
			{
				if (ancestor is ILanguage)
				{
					ILanguage language = ancestor as ILanguage;
					if (string.IsNullOrEmpty(language.LanguageCode) || !ancestor.Visible)
                        continue;
                    return language;
				}
			}
			return null;
		}

        /// <summary>Gets the language with a certain language code.</summary>
        /// <param name="languageCode">The language code to find a matching language for.</param>
        /// <returns>The language with the code or null.</returns>
        public ILanguage GetLanguage(string languageCode)
        {
            if(languageCode == null) throw new ArgumentNullException("languageCode");

            foreach (ILanguage language in GetAvailableLanguages())
            {
                if (languageCode.Equals(language.LanguageCode, StringComparison.InvariantCultureIgnoreCase))
                    return language;
            }

            return null;
        }

		public IEnumerable<ILanguage> GetAvailableLanguages()
		{
			foreach (ILanguage language in new RecursiveFinder().Find<ILanguage>(persister.Get(host.CurrentSite.RootItemID), RecursionDepth, typeof(ITrashCan)))
			{
				if (!string.IsNullOrEmpty(language.LanguageCode) && (language as ContentItem).Visible)
				{
					yield return language;
				}
			}
		}

		public IEnumerable<ContentItem> FindTranslations(ContentItem item)
		{
            if (item == null) 
                return new ContentItem[0];

			if (item is ILanguage)
			{
				List<ContentItem> languages = new List<ContentItem>();
				foreach (ILanguage language in GetAvailableLanguages())
				{
					languages.Add(language as ContentItem);
				}
				return languages;
			}

			if (item[LanguageKey] == null)
				return new ContentItem[0];

			int key = (int)item[LanguageKey];
			return finder.Where.Detail(LanguageKey).Eq(key).Filters(new AccessFilter(context.User, security)).Select();
		}

		public IEnumerable<TranslateSpecification> GetEditTranslations(ContentItem item, bool includeCurrent)
		{
			ILanguage itemlanguage = GetLanguage(item);
			if (itemlanguage == null)
				yield break;

			IEnumerable<ContentItem> translations = FindTranslations(item);
			IEnumerable<ILanguage> languages = GetAvailableLanguages();

			foreach (ILanguage language in languages)
			{
				if (language != itemlanguage || includeCurrent)
				{
					ContentItem translation = GetTranslation(translations, language);
					if (translation == null && language == itemlanguage)
						translation = item;

					ItemDefinition definition = definitions.GetDefinition(item.GetType());
					if (translation != null)
					{
						string url = editManager.GetEditExistingItemUrl(translation);
						yield return new TranslateSpecification(url, language, translation, definition);
					}
					else
					{
						ContentItem translatedParent = GetTranslatedParent(item, language);

						if (translatedParent == null)
							continue;

						Url url = editManager.GetEditNewPageUrl(translatedParent, definition, item.ZoneName, CreationPosition.Below);
					    url = url.AppendQuery(LanguageKey, item[LanguageKey] ?? item.ID);

						yield return new TranslateSpecification(url, language, translation, definition);
					}
				}
			}
		}

		private ContentItem GetTranslatedParent(ContentItem item, ILanguage language)
		{
			ContentItem parent = item.Parent;
			IEnumerable<ContentItem> translationsOfParent = FindTranslations(parent);
			ContentItem translatedParent = GetTranslation(translationsOfParent, language);
			return translatedParent;
		}

		public ContentItem GetTranslation(ContentItem item, ILanguage language)
		{
			return GetTranslation(FindTranslations(item), language);
		}

		private ContentItem GetTranslation(IEnumerable<ContentItem> translations, ILanguage language)
		{
			foreach (ContentItem translation in translations)
			{
				ILanguage translationLanguage = GetLanguage(translation);
				if (language == translationLanguage)
					return translation;
			}
			return null;
		}

        /// <summary>Associate these items from different language branches as the same translated page. If a language branch already contains an associated item that item will be de-associated and be removed as a translation.</summary>
        /// <param name="items">The translations to associate with each other.</param>
        public void Associate(IEnumerable<ContentItem> items)
        {
            EnsureNoLanguageRoots(items);

            ContentItem appointedMaster = AppointMaster(items);
            foreach (ContentItem itemToSave in UpdateLanguageKeys(items, appointedMaster))
            {
                persister.Save(itemToSave);
            }
        }

        /// <summary>
        /// Unassociates an item from the relation to other translated pages.
        /// </summary>
        /// <param name="item">The item to remove as translation.</param>
        public void Unassociate(ContentItem item)
        {
            item[LanguageKey] = null;
            persister.Save(item);
        }

        /// <summary>Throws an exception if any of the items is a language root.</summary>
        /// <param name="items"></param>
        protected void EnsureNoLanguageRoots(IEnumerable<ContentItem> items)
        {
            foreach (ContentItem item in items)
                if (item == GetLanguage(item))
                    throw new N2Exception("Cannot associate " + item.Name + " #" + item.ID + " since it's a language root.");
        }

        /// <summary>Determines the preferred "master" or picks the first one.</summary>
        /// <param name="items"></param>
        /// <returns>An item whose id will be used as language key.</returns>
        protected ContentItem AppointMaster(IEnumerable<ContentItem> items)
        {
            ContentItem appointedMaster = null;
            foreach (ContentItem item in items)
            {
                appointedMaster = item;
                if (appointedMaster[LanguageKey] != null)
                    break;
            }
            foreach (ContentItem item in items)
            {
                if (item[LanguageGateway.LanguageKey] != null && item.ID == (int)item[LanguageGateway.LanguageKey])
                {
                    appointedMaster = item;
                    break;
                }
            }
            if (appointedMaster[LanguageGateway.LanguageKey] == null)
                appointedMaster[LanguageGateway.LanguageKey] = appointedMaster.ID;
            return appointedMaster;
        }

        /// <summary>Updates language keys and removes language keys from any existing translations within a certain language branch.</summary>
        /// <param name="items"></param>
        /// <param name="appointedMaster"></param>
        /// <returns>Items that have been updated as a result of the operation.</returns>
        protected IEnumerable<ContentItem> UpdateLanguageKeys(IEnumerable<ContentItem> items, ContentItem appointedMaster)
        {
            foreach (ContentItem item in items)
            {
                ILanguage language = GetLanguage(item);
                ContentItem existingTranslation = GetTranslation(appointedMaster, language);
                if (item == existingTranslation)
                {
                    continue;
                }
                else if (existingTranslation != null)
                {
                    existingTranslation[LanguageGateway.LanguageKey] = null;
                    yield return existingTranslation;
                }
                item[LanguageGateway.LanguageKey] = appointedMaster[LanguageGateway.LanguageKey];
                yield return item;
            }
        }
    }
}
