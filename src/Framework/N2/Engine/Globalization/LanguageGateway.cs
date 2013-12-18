using System;
using System.Linq;
using System.Collections.Generic;
using N2.Collections;
using N2.Configuration;
using N2.Definitions;
using N2.Edit;
using N2.Edit.Trash;
using N2.Persistence;
using N2.Persistence.Finder;
using N2.Security;
using N2.Web;

namespace N2.Engine.Globalization
{
    /// <summary>
    /// Globalization handler.
    /// </summary>
    [Service(typeof(ILanguageGateway))]
    public class LanguageGateway : ILanguageGateway
    {
        public const string TranslationKey = "TranslationKey";

        readonly IPersister persister;
        readonly IEditUrlManager editUrlManager;
        readonly IDefinitionManager definitions;
        readonly IHost host;
        readonly ISecurityManager security;
        readonly IWebContext context;
        readonly StructureBoundDictionaryCache<int, LanguageInfo[]> languagesCache;
        readonly DescendantItemFinder descendantFinder;
        bool enabled = true;

        public LanguageGateway(
            IPersister persister,
            IEditUrlManager editUrlManager,
            IDefinitionManager definitions,
            IHost host,
            ISecurityManager security,
            IWebContext context,
            StructureBoundDictionaryCache<int, LanguageInfo[]> languagesCache,
            DescendantItemFinder descendantFinder,
            EngineSection config)
        {
            this.persister = persister;
            this.editUrlManager = editUrlManager;
            this.definitions = definitions;
            this.host = host;
            this.security = security;
            this.context = context;
            this.languagesCache = languagesCache;
            this.descendantFinder = descendantFinder;
            Enabled = config.Globalization.Enabled;
        }

        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        public ILanguage GetLanguage(ContentItem item)
        {
            var infos = GetLanguageInfos();
            foreach (ContentItem ancestor in Find.EnumerateParents(item, lastAncestor: null, includeSelf: true))
            {
                if (ancestor is ILanguage)
                {
                    var info = infos.FirstOrDefault(li => li.ID == ancestor.ID);
                    if (info != null)
                        return info;
                }
            }
            return null;
        }

        /// <summary>Gets the language with a certain language code.</summary>
        /// <param name="languageCode">The language code to find a matching language for.</param>
        /// <returns>The language with the code or null.</returns>
        public ILanguage GetLanguage(string languageCode)
        {
            return GetLanguageWithCode(GetAvailableLanguages(), languageCode);
        }

        public IEnumerable<ILanguage> GetAvailableLanguages()
        {
            var languages = GetLanguageInfos();
            foreach(var language in languages)
            {
                yield return language;
            }
        }

        private LanguageInfo[] GetLanguageInfos()
        {
            return languagesCache.GetValue(host.CurrentSite.RootItemID, FindLanguagesRecursive);
        }

        private LanguageInfo[] FindLanguagesRecursive(int rootNodeID)
        {
            List<LanguageInfo> languages = new List<LanguageInfo>();
            foreach (ILanguage language in descendantFinder.Find<ILanguage>(persister.Get(rootNodeID)))
            {
                if (!string.IsNullOrEmpty(language.LanguageCode))
                {
                    var languageItem = language as ContentItem;
                    int languageID = languageItem.ID;
                    if (languageItem != null && languageItem.ID > 0)
                        languages.Add(new LanguageInfo { ID = languageID, LanguageCode = language.LanguageCode, LanguageTitle = language.LanguageTitle, Translation = () => persister.Get(languageID) });
                }
            }
            return languages.ToArray();
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
                    languages.Add(GetTranslation(language));
                }
                return languages;
            }

            if (item.TranslationKey == null)
                return new ContentItem[0];

            return persister.Repository.Find(TranslationKey, item.TranslationKey)
                .Where(new AccessFilter(context.User, security))
                .Where(Content.Is.Not(Content.Is.DescendantOf<ITrashCan>()))
                ;//.Where(i => i.TranslationKey == item.TranslationKey); // filtering again since translation key might have been cleared in the same transaction
        }

        public IEnumerable<TranslateSpecification> GetEditTranslations(ContentItem item, bool includeCurrent, bool generateNonTranslated)
        {
            ILanguage itemlanguage = GetLanguage(item);
            if (itemlanguage == null)
                yield break;

            IEnumerable<ContentItem> translations = FindTranslations(item);
            IEnumerable<ILanguage> languages = GetAvailableLanguages();

            var itemSite = host.GetSite(item);
            foreach (ILanguage language in languages)
            {
                if (language != itemlanguage || includeCurrent)
                {
                    ContentItem translation = GetTranslation(translations, language);
                    if (translation == null && language == itemlanguage)
                        translation = item;

                    ItemDefinition definition = definitions.GetDefinition(item);
                    if (translation != null)
                    {
                        string url = editUrlManager.GetEditExistingItemUrl(translation);
                        yield return new TranslateSpecification(url, language, translation, definition, host.GetSite(translation));
                    }
                    else
                    {
                        ContentItem translatedParent = GetTranslatedParent(item, language);

                        if (translatedParent == null)
                        {
                            if (generateNonTranslated)
                            {
                                yield return new TranslateSpecification("#", language, translation, definition, host.GetSite(GetTranslation(language)))
                                {
                                    IsTranslatable = false
                                };
                            }
                            continue;
                        }

                        Url url = editUrlManager.GetEditNewPageUrl(translatedParent, definition, item.ZoneName, CreationPosition.Below);
                        url = url.AppendQuery(TranslationKey, item.TranslationKey ?? item.ID);

                        yield return new TranslateSpecification(url, language, translation, definition, host.GetSite(translatedParent));
                    }
                }
            }
        }

        private ContentItem GetTranslation(ILanguage language)
        {
            if (language is LanguageInfo)
                return (language as LanguageInfo).Translation();
            else if (language is ContentItem)
                return language as ContentItem;
            else
                throw new InvalidOperationException("Can't get translation of " + language);
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
            using (var tx = persister.Repository.BeginTransaction())
            {
                foreach (ContentItem itemToSave in UpdateLanguageKeys(items, appointedMaster))
                {
                    persister.Repository.SaveOrUpdate(itemToSave);
                }
                tx.Commit();
            }
        }

        /// <summary>
        /// Unassociates an item from the relation to other translated pages.
        /// </summary>
        /// <param name="item">The item to remove as translation.</param>
        public void Unassociate(ContentItem item)
        {
            if (item.TranslationKey != null)
            {
                item.TranslationKey = null;
                using (var tx = persister.Repository.BeginTransaction())
                {
                    persister.Repository.SaveOrUpdate(item);
                    tx.Commit();
                }
            }
        }

        /// <summary>Gets indication whether a certain item is the root item of a language branch.</summary>
        /// <param name="item">The item to check.</param>
        /// <returns>True if the item is the root item of a language branch.</returns>
        public bool IsLanguageRoot(ContentItem item)
        {
            return item is ILanguage && !string.IsNullOrEmpty((item as ILanguage).LanguageCode);
        }

        /// <summary>Throws an exception if any of the items is a language root.</summary>
        /// <param name="items"></param>
        protected void EnsureNoLanguageRoots(IEnumerable<ContentItem> items)
        {
            foreach (ContentItem item in items)
                if (object.ReferenceEquals(item, GetLanguage(item)))
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
                if (appointedMaster.TranslationKey != null)
                    break;
            }
            foreach (ContentItem item in items)
            {
                if (item.TranslationKey != null && item.ID == (int)item.TranslationKey)
                {
                    appointedMaster = item;
                    break;
                }
            }
            if (appointedMaster.TranslationKey == null)
                appointedMaster.TranslationKey = appointedMaster.ID;
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
                    existingTranslation.TranslationKey = null;
                    yield return existingTranslation;
                }
                item.TranslationKey = appointedMaster.TranslationKey;
                yield return item;
            }
        }

        /// <summary>Gets the language with a certain language code.</summary>
        /// <param name="languages">The languages to select between.</param>
        /// <param name="languageCode">The language code to find a matching language for.</param>
        /// <returns>The language with the code or null.</returns>
        public static ILanguage GetLanguageWithCode(IEnumerable<ILanguage> languages, string languageCode)
        {
            if (languageCode == null) throw new ArgumentNullException("languageCode");

            return languages.FirstOrDefault(l => languageCode.Equals(l.LanguageCode, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
