using System;
using System.Collections.Generic;
using System.Text;
using N2.Persistence;
using N2.Web;
using N2.Collections;
using Castle.Core;
using N2.Persistence.Finder;
using N2.Edit;
using N2.Definitions;
using N2.Security;

namespace N2.Globalization
{
	public class LanguageGateway : ILanguageGateway
	{
		public const string LanguageKey = "LanguageKey";

		private readonly IPersister persister;
		private readonly IItemFinder finder;
		private readonly IEditManager editManager;
		private readonly IDefinitionManager definitions;
		private readonly IHost host;
		private int recursionDepth = 3;
        ISecurityManager security;
        IWebContext context;

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
					return ancestor as ILanguage;
				}
			}
			return null;
		}

		public IEnumerable<ILanguage> GetAvailableLanguages()
		{
			foreach (ILanguage language in new RecursiveFinder().Find<ILanguage>(persister.Get(host.DefaultSite.RootItemID), RecursionDepth))
			{
				if (!string.IsNullOrEmpty(language.LanguageCode))
				{
					yield return language;
				}
			}
		}

		public IEnumerable<ContentItem> FindTranslations(ContentItem item)
		{
			if (item == null) throw new ArgumentNullException("item");
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

						string url = editManager.GetEditNewPageUrl(translatedParent, definition, item.ZoneName, CreationPosition.Below);
						url += "&" + LanguageKey + "=" + (item[LanguageKey] ?? item.ID);

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
	}
}
