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

        public bool Enabled
        {
            get { return languages.Enabled; }
        }

        public IEnumerable<ContentItem> FindTranslations(ContentItem item)
        {
            return languages.FindTranslations(item);
        }

        public ILanguage GetLanguage(ContentItem item)
        {
            return languages.GetLanguage(item);
        }

        public ILanguage GetLanguage(string languageCode)
        {
            return LanguageGateway.GetLanguageWithCode(GetAvailableLanguages(), languageCode);
        }

        public IEnumerable<TranslateSpecification> GetEditTranslations(ContentItem item, bool includeCurrent, bool generateNonTranslated)
        {
            return languages.GetEditTranslations(item, includeCurrent, generateNonTranslated).Where(ts => ts.Site == site);
        }

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

        public ContentItem GetTranslation(ContentItem item, ILanguage language)
        {
            return languages.GetTranslation(item, language);
        }

        public void Associate(IEnumerable<ContentItem> items)
        {
            languages.Associate(items);
        }

        public void Unassociate(ContentItem item)
        {
            languages.Unassociate(item);
        }
    }
}
