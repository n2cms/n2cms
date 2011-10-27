using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Persistence;
using N2.Edit;
using N2.Definitions;
using N2.Web;
using N2.Security;
using N2.Configuration;

namespace N2.Engine.Globalization
{
    /// <summary>
    /// Provides a way to filter languages based on the containing site.
    /// </summary>
    [Service]
    public class LanguageGatewaySelector
    {
		readonly IPersister persister;
		readonly IHost host;
		readonly StructureBoundDictionaryCache<int, LanguageInfo[]> languagesCache;
		readonly DescendantItemFinder descendantFinder;
        private ILanguageGateway languages;

        /// <summary>True if the language feature is enabled in web.config.</summary>
        public bool Enabled { get; protected set; }

        /// <summary>True if the language per site feature is enabled in web.config.</summary>
        public bool LanguagesPerSite { get; protected set; }

        public LanguageGatewaySelector(
			IPersister persister,
			IHost host,
			StructureBoundDictionaryCache<int, LanguageInfo[]> languagesCache,
			DescendantItemFinder descendantFinder,
            ILanguageGateway languages,
			EngineSection config)
		{
			this.persister = persister;
			this.host = host;
			this.languagesCache = languagesCache;
			this.descendantFinder = descendantFinder;
            this.languages = languages;
			Enabled = config.Globalization.Enabled;
            LanguagesPerSite = config.Globalization.LanguagesPerSite;
		}

        /// <summary>Gets the language gateway manaing translations on the site for the given item.</summary>
        /// <param name="managingTranslationsOfItem">The item whose related language gateway to get.</param>
        /// <returns>A language gateway filtering languages.</returns>
        public ILanguageGateway GetLanguageGateway(ContentItem managingTranslationsOfItem)
        {
            return GetLanguageGateway(host.GetSite(managingTranslationsOfItem));
        }

        /// <summary>Gets the language gateway manaing translations on the given site.</summary>
        /// <param name="managingTranslationsOnSite">the site whose language gateway to get.</param>
        /// <returns>A language gateway filtering languages.</returns>
        public ILanguageGateway GetLanguageGateway(Site managingTranslationsOnSite)
        {
            if (Enabled && LanguagesPerSite)
                return new SiteFilteringLanguageGateway(languages, managingTranslationsOnSite, persister, languagesCache, descendantFinder);

            return languages;
        }

        /// <summary>Gets the language gateway manaing translations on the current site.</summary>
        /// <returns>A language gateway filtering languages.</returns>
        public ILanguageGateway GetLanguageGateway()
        {
            return GetLanguageGateway(host.CurrentSite);
        }

        /// <summary>
        /// Gets a language gateway that doesn't filter languages to the current site.
        /// </summary>
        /// <returns></returns>
        public ILanguageGateway GetAllLanguages()
        {
            return languages;
        }
    }
}
