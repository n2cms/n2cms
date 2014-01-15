using System.Collections.Generic;

namespace N2.Engine.Globalization
{
    /// <summary>
    /// Globalization functionality.
    /// </summary>
    public interface ILanguageGateway
    {
        /// <summary>Get wether the language functionality is enabled.</summary>
        bool Enabled { get; }

        /// <summary>Gets available translations for a certain item.</summary>
        /// <param name="item">The item whose translations to get..</param>
        /// <returns>An enumeration of content itmes that are translations of the given item. The item itself is included.</returns>
        IEnumerable<ContentItem> FindTranslations(ContentItem item);
        
        /// <summary>Gets the language of an item.</summary>
        /// <param name="item">The item whose language to get.</param>
        /// <returns>The item's language or null.</returns>
        ILanguage GetLanguage(ContentItem item);

        /// <summary>Gets the language with a certain language code.</summary>
        /// <param name="languageCode">The language code to find a matching language for.</param>
        /// <returns>The language with the code or null.</returns>
        ILanguage GetLanguage(string languageCode);

        /// <summary>Gets an enumeration of translation options for edit mode.</summary>
        /// <param name="item">The item beeing edited.</param>
        /// <param name="includeCurrent">Wether to include the current item in the enumeratin.</param>
        /// <param name="generateNonTranslated"></param>
        /// <returns>An enumeration of translation definitions.</returns>
        IEnumerable<TranslateSpecification> GetEditTranslations(ContentItem item, bool includeCurrent, bool generateNonTranslated);

        /// <summary>Gets available languages.</summary>
        /// <returns>An enumeration of available languages.</returns>
        IEnumerable<ILanguage> GetAvailableLanguages();
        
        /// <summary>Gets an item's translation in a certain language.</summary>
        /// <param name="item">The item whose translation to get.</param>
        /// <param name="language">The language to look for.</param>
        /// <returns>A translated item or null.</returns>
        ContentItem GetTranslation(ContentItem item, ILanguage language);

        /// <summary>Associate these items from different language branches as the same translated page. If a language branch already contains an associated item that item will be de-associated and be removed as a translation.</summary>
        /// <param name="items">The translations to associate with each other.</param>
        void Associate(IEnumerable<ContentItem> items);

        /// <summary>Unassociates an item from the relation to other translated pages.</summary>
        /// <param name="item">The item to remove as translation.</param>
        void Unassociate(ContentItem item);

        /// <summary>Gets indication whether a certain item is the root item of a language branch.</summary>
        /// <param name="item">The item to check.</param>
        /// <returns>True if the item is the root item of a language branch.</returns>
        bool IsLanguageRoot(ContentItem item);
    }
}
