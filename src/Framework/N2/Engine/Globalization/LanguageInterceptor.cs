using N2.Configuration;
using N2.Persistence;
using N2.Plugin;
using N2.Web;
using System.Web;

namespace N2.Engine.Globalization
{
    /// <summary>
    /// Intercepts and acts upon operations on the node tree. The purpose is to 
    /// keep the language branches synchronized.
    /// </summary>
    [Service]
    public class LanguageInterceptor : IAutoStart
    {
        private const string DeletingKey = "LanguageInterceptor_Deleting";

        readonly IPersister persister;
        readonly ContentActivator activator;
        readonly IWebContext context;
        readonly ILanguageGateway gateway;
        readonly bool enabled = true;
        readonly bool autoDeleteTranslations;

        public LanguageInterceptor(IPersister persister, ContentActivator activator, IWebContext context, ILanguageGateway gateway, EngineSection config)
            : this(persister, activator, context, gateway)
        {
            enabled = config.Globalization.Enabled;
            autoDeleteTranslations = config.Globalization.AutoDeleteTranslations;
        }

        public LanguageInterceptor(IPersister persister, ContentActivator activator, IWebContext context, ILanguageGateway gateway)
        {
            this.persister = persister;
            this.activator = activator;
            this.context = context;
            this.gateway = gateway;
        }


        void definitions_ItemCreated(object sender, ItemEventArgs e)
        {
            if (GetLanguageKey() != null)
            {
                if (e.AffectedItem is ILanguage)
                    return;

                UpdateSortOrder(e.AffectedItem);
            }
        }

        private string GetLanguageKey()
        {
            return context.Url.LocalUrl.GetQuery(LanguageGateway.TranslationKey);
        }

        private void UpdateSortOrder(ContentItem item)
        {
            int languageKey;
            if (int.TryParse(GetLanguageKey(), out languageKey))
            {
                ContentItem translation = persister.Get(languageKey);
                if (translation != null)
                    item.SortOrder = translation.SortOrder;
            }
        }

        void persister_ItemCopied(object sender, DestinationEventArgs e)
        {
            e.AffectedItem.TranslationKey = null;
            persister.Save(e.AffectedItem);
        }

        void persister_ItemDeleting(object sender, CancellableItemEventArgs e)
        {
            if (!autoDeleteTranslations)
            {
                gateway.Unassociate(e.AffectedItem);
                return;
            }

            // prevent infinite recursion
            if (context.RequestItems[DeletingKey] != null) return;

            ContentItem item = e.AffectedItem;
            using (new DictionaryScope(context.RequestItems, DeletingKey, item))
            {
                if (item is ILanguage)
                    return;

                DeleteTranslations(item);
            }
        }

        private void DeleteTranslations(ContentItem item)
        {
            foreach (ContentItem translatedItem in gateway.FindTranslations(item))
            {
                if (translatedItem != item)
                {
                    persister.Delete(translatedItem);
                }
            }
        }

        void persister_ItemMoved(object sender, DestinationEventArgs e)
        {
            ContentItem item = e.AffectedItem;
            if (item is ILanguage)
                return;
            ILanguage language = gateway.GetLanguage(item);

            if (language != null)
            {
                ContentItem destination = e.Destination;

                MoveTranslations(item, language, destination);
            }
        }

        private void MoveTranslations(ContentItem item, ILanguage language, ContentItem destination)
        {
            foreach (ContentItem translatedItem in gateway.FindTranslations(item))
            {
                ILanguage translationsLanguage = gateway.GetLanguage(translatedItem);
                ContentItem translatedDestination = gateway.GetTranslation(destination, translationsLanguage);
                if (translationsLanguage != language && translatedDestination != null && translatedItem.Parent != translatedDestination)
                {
                    persister.Move(translatedItem, translatedDestination);
                }
            }
        }

        void persister_ItemSaved(object sender, ItemEventArgs e)
        {
            try
            {
                ContentItem item = e.AffectedItem;
                ILanguage language = gateway.GetLanguage(item);
                if (language != null && !string.IsNullOrEmpty(language.LanguageCode))
                {
                    UpdateLanguageKey(item);
                }
            }
            catch (System.Exception)
            {
                
                throw;
            }
        }

        private void UpdateLanguageKey(ContentItem item)
        {
            int languageKey = item.ID;
            string languageKeyString = GetLanguageKey();
            if (languageKeyString != null)
            {
                int.TryParse(languageKeyString, out languageKey);
                if (item.TranslationKey == null)
                {
                    item.TranslationKey = languageKey;
                    persister.Save(item);

                    if (languageKey != item.ID)
                    {
                        EnsureLanguageKeyOnInitialTranslation(item, languageKey);
                    }
                }
            }
        }

        private void EnsureLanguageKeyOnInitialTranslation(ContentItem item, int languageKey)
        {
            ContentItem initialTranslation = persister.Get(languageKey);
            if (initialTranslation == null || initialTranslation.TranslationKey != null)
                return;

            initialTranslation.TranslationKey = languageKey;
            persister.Save(item);
        }

        #region IStartable Members

        public void Start()
        {
            if (!enabled)
                return;

            persister.ItemSaved += persister_ItemSaved;
            persister.ItemMoved += persister_ItemMoved;
            persister.ItemDeleting += persister_ItemDeleting;
            activator.ItemCreated += definitions_ItemCreated;
            persister.ItemCopied += persister_ItemCopied;
        }

        public void Stop()
        {
            if (!enabled)
                return;

            persister.ItemSaved -= persister_ItemSaved;
            persister.ItemMoved -= persister_ItemMoved;
            persister.ItemDeleting -= persister_ItemDeleting;
            activator.ItemCreated -= definitions_ItemCreated;
            persister.ItemCopied -= persister_ItemCopied;
        }

        #endregion
    }
}
