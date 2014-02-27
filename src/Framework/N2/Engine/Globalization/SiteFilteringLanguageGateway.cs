using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Web;
using N2.Persistence;

namespace N2.Engine.Globalization
{
    /// <summary>
    /// Supports <see cref="LanguageGatewaySelector"/> in filtering languages based on site location.
    /// </summary>
    public class SiteFilteringLanguageGateway : ILanguageGateway
    {
        private ILanguageGateway languages;
        private Site site;
        private IPersister persister;
        private StructureBoundDictionaryCache<int, LanguageInfo[]> languagesCache;
        private DescendantItemFinder descendantFinder;

        public SiteFilteringLanguageGateway(ILanguageGateway languages, Site site, IPersister persister, StructureBoundDictionaryCache<int, LanguageInfo[]> languagesCache, DescendantItemFinder descendantFinder)
        {
            this.languages = languages;
            this.site = site;
            this.persister = persister;
            this.languagesCache = languagesCache;
            this.descendantFinder = descendantFinder;
        }

        /// <summary>Get wether the language functionality is enabled.</summary>
        public bool Enabled
        {
            get { return languages.Enabled; }
        }

        /// <summary>Gets available translations for a certain item.</summary>
        /// <param name="item">The item whose translations to get..</param>
        /// <returns>An enumeration of content itmes that are translations of the given item. The item itself is included.</returns>
        public IEnumerable<ContentItem> FindTranslations(ContentItem item)
        {
            if (item is ILanguage)
                return GetAvailableLanguages().Select(l => l as ContentItem);

            return languages.FindTranslations(item);
        }

        /// <summary>Gets the language of an item.</summary>
        /// <param name="item">The item whose language to get.</param>
        /// <returns>The item's language or null.</returns>
        public ILanguage GetLanguage(ContentItem item)
        {
            return languages.GetLanguage(item);
        }

        /// <summary>Gets the language with a certain language code.</summary>
        /// <param name="languageCode">The language code to find a matching language for.</param>
        /// <returns>The language with the code or null.</returns>
        public ILanguage GetLanguage(string languageCode)
        {
            return LanguageGateway.GetLanguageWithCode(GetAvailableLanguages(), languageCode);
        }

        /// <summary>Gets an enumeration of translation options for edit mode.</summary>
        /// <param name="item">The item beeing edited.</param>
        /// <param name="includeCurrent">Wether to include the current item in the enumeratin.</param>
        /// <returns>An enumeration of translation definitions.</returns>
        public IEnumerable<TranslateSpecification> GetEditTranslations(ContentItem item, bool includeCurrent, bool generateNonTranslated)
        {
            return languages.GetEditTranslations(item, includeCurrent, generateNonTranslated).Where(ts => ts.Site == site);
        }

        /// <summary>Gets available languages.</summary>
        /// <returns>An enumeration of available languages.</returns>
        public IEnumerable<ILanguage> GetAvailableLanguages()
        {
            var languages = languagesCache.GetValue(site.StartPageID, FindLanguagesRecursive);
            foreach (var language in languages)
            {
                yield return persister.Get(language.ID) as ILanguage;
            }
        }

        private LanguageInfo[] FindLanguagesRecursive(int rootNodeID)
        {
            List<LanguageInfo> languages = new List<LanguageInfo>();
            foreach (ILanguage language in descendantFinder.Find<ILanguage>(persister.Get(rootNodeID)))
            {
                if (!string.IsNullOrEmpty(language.LanguageCode))
                {
                    var languageItem = language as ContentItem;
                    if (languageItem != null && languageItem.ID > 0)
                        languages.Add(new LanguageInfo { ID = languageItem.ID, LanguageCode = language.LanguageCode });
                }
            }
            return languages.ToArray();
        }

        /// <summary>Gets an item's translation in a certain language.</summary>
        /// <param name="item">The item whose translation to get.</param>
        /// <param name="language">The language to look for.</param>
        /// <returns>A translated item or null.</returns>
        public ContentItem GetTranslation(ContentItem item, ILanguage language)
        {
            return languages.GetTranslation(item, language);
        }

        /// <summary>Associate these items from different language branches as the same translated page. If a language branch already contains an associated item that item will be de-associated and be removed as a translation.</summary>
        /// <param name="items">The translations to associate with each other.</param>
        public void Associate(IEnumerable<ContentItem> items)
        {
            languages.Associate(items);
        }

        /// <summary>Unassociates an item from the relation to other translated pages.</summary>
        /// <param name="item">The item to remove as translation.</param>
        public void Unassociate(ContentItem item)
        {
            languages.Unassociate(item);
        }

        /// <summary>Gets indication whether a certain item is the root item of a language branch.</summary>
        /// <param name="item">The item to check.</param>
        /// <returns>True if the item is the root item of a language branch.</returns>
        public bool IsLanguageRoot(ContentItem item)
        {
            return languages.IsLanguageRoot(item);
        }
    }
}
