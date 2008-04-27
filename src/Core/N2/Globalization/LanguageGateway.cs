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

namespace N2.Globalization
{
	public class LanguageGateway : ILanguageGateway, IStartable
	{
		public const string LanguageKey = "LanguageKey";

		private readonly IPersister persister;
		private readonly IWebContext context;
		private readonly IItemFinder finder;
		private readonly IEditManager editManager;
		private readonly IDefinitionManager definitions;
		private readonly Site site;
		private int recursionDepth = 3;
		
		public int RecursionDepth
		{
			get { return recursionDepth; }
			set { recursionDepth = value; }
		}

		public LanguageGateway(IPersister persister, IWebContext context, IItemFinder finder, IEditManager editManager, IDefinitionManager definitions, Site site)
		{
			this.persister = persister;
			this.context = context;
			this.finder = finder;
			this.editManager = editManager;
			this.definitions = definitions;
			this.site = site;
		}

		private ILanguageRoot GetLanguageAncestor(ContentItem item)
		{
			foreach (ContentItem ancestor in Find.EnumerateParents(item, null, true))
			{
				if (ancestor is ILanguageRoot)
				{
					return ancestor as ILanguageRoot;
				}
			}
			return null;
		}

		void persister_ItemSaved(object sender, ItemEventArgs e)
		{
			ContentItem item = e.AffectedItem;
			ILanguageRoot language = GetLanguageAncestor(item);
			if(language != null)
			{
				int languageKey = item.ID;
				if (context.QueryString[LanguageKey] != null)
					int.TryParse(context.QueryString[LanguageKey], out languageKey);
				if (item[LanguageKey] == null)
				{
					item[LanguageKey] = languageKey;
					persister.Save(item);
				}
			}
		}

		public IEnumerable<ILanguageRoot> GetLanguages()
		{
			return new RecursiveFinder().Find<ILanguageRoot>(persister.Get(site.RootItemID), RecursionDepth);
		}

		public IEnumerable<ContentItem> FindTranslations(ContentItem item)
		{
			if (item == null) throw new ArgumentNullException("item");
			if (item is ILanguageRoot)
			{
				List<ContentItem> languages = new List<ContentItem>();
				foreach (ILanguageRoot language in GetLanguages())
				{
					languages.Add(language as ContentItem);
				}
				return languages;
			}

			if (item[LanguageKey] == null)
				return new ContentItem[0];

			int key = (int)item[LanguageKey];
			return finder.Where.Detail(LanguageKey).Eq(key).Select();
		}

		public IEnumerable<TranslationOption> GetTranslationOptions(ContentItem item)
		{
			ILanguageRoot itemlanguage = GetLanguageAncestor(item);
			if (itemlanguage == null)
				yield break;

			IEnumerable<ContentItem> translations = FindTranslations(item);
			IEnumerable<ILanguageRoot> languages = GetLanguages();

			foreach (ILanguageRoot language in languages)
			{
				if (language != itemlanguage)
				{
					ContentItem translation = GetTranslation(translations, language);
					if (translation != null)
					{
						yield return new TranslationOption(editManager.GetEditExistingItemUrl(translation), false, language);
					}
					else
					{
						ContentItem translatedParent = GetTranslatedParent(item, language);
						string url = GetCreateNewUrl(item, translatedParent);
						yield return new TranslationOption(url, true, language);
					}
				}
			}
		}

		private string GetCreateNewUrl(ContentItem item, ContentItem translatedParent)
		{
			ItemDefinition definition = definitions.GetDefinition(item.GetType());
			string url = editManager.GetEditNewPageUrl(translatedParent, definition, null, CreationPosition.Below);
			url += "&" + LanguageKey + "=" + item.ID;
			return url;
		}

		private ContentItem GetTranslatedParent(ContentItem item, ILanguageRoot language)
		{
			ContentItem parent = item.Parent;
			IEnumerable<ContentItem> translationsOfParent = FindTranslations(parent);
			ContentItem translatedParent = GetTranslation(translationsOfParent, language);
			return translatedParent;
		}

		private ContentItem GetTranslation(IEnumerable<ContentItem> translations, ILanguageRoot language)
		{
			foreach (ContentItem translation in translations)
			{
				ILanguageRoot translationLanguage = GetLanguageAncestor(translation);
				if (language == translationLanguage)
					return translation;
			}
			return null;
		}

		#region IStartable Members

		public void Start()
		{
			persister.ItemSaved += persister_ItemSaved;
		}

		public void Stop()
		{
			persister.ItemSaved -= persister_ItemSaved; ;
		}

		#endregion
	}
}
