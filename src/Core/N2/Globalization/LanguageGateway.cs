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
	public class LanguageGateway : ILanguageGateway
	{
		public const string LanguageKey = "LanguageKey";

		private readonly IPersister persister;
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

		public LanguageGateway(IPersister persister, IItemFinder finder, IEditManager editManager, IDefinitionManager definitions, Site site)
		{
			this.persister = persister;
			this.finder = finder;
			this.editManager = editManager;
			this.definitions = definitions;
			this.site = site;
		}

		public ILanguage GetLanguageAncestor(ContentItem item)
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

		public IEnumerable<ILanguage> GetLanguages()
		{
			return new RecursiveFinder().Find<ILanguage>(persister.Get(site.RootItemID), RecursionDepth);
		}

		public IEnumerable<ContentItem> FindTranslations(ContentItem item)
		{
			if (item == null) throw new ArgumentNullException("item");
			if (item is ILanguage)
			{
				List<ContentItem> languages = new List<ContentItem>();
				foreach (ILanguage language in GetLanguages())
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

		public IEnumerable<TranslationOption> GetTranslationOptions(ContentItem item, bool includeCurrent)
		{
			ILanguage itemlanguage = GetLanguageAncestor(item);
			if (itemlanguage == null)
				yield break;

			IEnumerable<ContentItem> translations = FindTranslations(item);
			IEnumerable<ILanguage> languages = GetLanguages();

			foreach (ILanguage language in languages)
			{
				if (language != itemlanguage || includeCurrent)
				{
					ContentItem translation = GetTranslation(translations, language);
					if (translation != null)
					{
						string url = editManager.GetEditExistingItemUrl(translation);
						yield return new TranslationOption(url, language, translation);
					}
					else
					{
						ContentItem translatedParent = GetTranslatedParent(item, language);
						string url = GetCreateNewUrl(item, translatedParent);
						yield return new TranslationOption(url, language, translation);
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
				ILanguage translationLanguage = GetLanguageAncestor(translation);
				if (language == translationLanguage)
					return translation;
			}
			return null;
		}
	}
}
