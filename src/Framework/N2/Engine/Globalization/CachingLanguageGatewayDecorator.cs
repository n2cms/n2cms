using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Web;
using N2.Web.UI;
using N2.Persistence;
using System.Web.Caching;

namespace N2.Engine.Globalization
{
    public class CachingLanguageGatewayDecorator : ILanguageGateway
    {
        private ILanguageGateway inner;
        private CacheWrapper cacheWrapper;
        private IPersister persister;

        TimeSpan slidingExpiration = TimeSpan.FromHours(1);
        private string masterKey;

        #region class LanguageCache
        class LanguageCache
        {
            public Dictionary<string, ILanguage> LanguageCodes { get; set; }

            public Dictionary<int, ILanguage> LanguageItems { get; set; }
        }
        #endregion

        #region class Language
        class Language : ILanguage
        {
            public Language(ILanguage language)
            {
                LanguageTitle = language.LanguageTitle;
                LanguageCode = language.LanguageCode;
            }

            public string LanguageTitle { get; set; }

            public string LanguageCode { get; set; }
        }
        #endregion

        public CachingLanguageGatewayDecorator(ILanguageGateway inner, CacheWrapper cacheWrapper, IPersister persister, string masterKey)
        {
            this.inner = inner;
            this.cacheWrapper = cacheWrapper;
            this.persister = persister;
            this.masterKey = masterKey;
        }

        public bool Enabled
        {
            get { return inner.Enabled; }
        }

        public IEnumerable<ContentItem> FindTranslations(ContentItem item)
        {
            return inner.FindTranslations(item);
        }

        public ILanguage GetLanguage(ContentItem item)
        {
            var cache = GetLanguageCache();

            ILanguage language;
            if (cache.LanguageItems.TryGetValue(item.ID, out language))
                return language;

            var innerLanguage = inner.GetLanguage(item);

            if (innerLanguage == null) return null;

            language = GetLanguage(innerLanguage.LanguageCode);
            cache.LanguageItems = new Dictionary<int, ILanguage>(cache.LanguageItems) { { item.ID, language } };
            
            return inner.GetLanguage(item);
        }

        public ILanguage GetLanguage(string languageCode)
        {
            var cache = GetLanguageCache();

            ILanguage language;
            if (cache.LanguageCodes.TryGetValue(languageCode, out language))
                return language;

            var innerLanguage = inner.GetLanguage(languageCode);

            if (innerLanguage == null) return null;

            language = new Language(innerLanguage);
            cache.LanguageCodes = new Dictionary<string, ILanguage>(cache.LanguageCodes) { { languageCode, language } };
            return language;
        }

        public IEnumerable<TranslateSpecification> GetEditTranslations(ContentItem item, bool includeCurrent, bool generateNonTranslated)
        {
            return inner.GetEditTranslations(item, includeCurrent, generateNonTranslated);
        }

        public IEnumerable<ILanguage> GetAvailableLanguages()
        {
            return inner.GetAvailableLanguages();
        }

        public ContentItem GetTranslation(ContentItem item, ILanguage language)
        {
            return inner.GetTranslation(item, language);
        }

        public void Associate(IEnumerable<ContentItem> items)
        {
            inner.Associate(items);
        }

        public void Unassociate(ContentItem item)
        {
            inner.Unassociate(item);
        }

        public bool IsLanguageRoot(ContentItem item)
        {
            return inner.IsLanguageRoot(item);
        }

        private LanguageCache GetLanguageCache()
        {
            var cache =  cacheWrapper.Get<LanguageCache>("CachingLanguageGatewayDecorator_" + masterKey);
            if (cache == null)
            {
                cache = new LanguageCache 
                { 
                    LanguageCodes = new Dictionary<string, ILanguage>(), 
                    LanguageItems = new Dictionary<int, ILanguage>() 
                };
                cacheWrapper.Add("CachingLanguageGatewayDecorator_" + masterKey, cache, new CacheOptions { SlidingExpiration = slidingExpiration });
            }
            return cache;
        }
    }
}
