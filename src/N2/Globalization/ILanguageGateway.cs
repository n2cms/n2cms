using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Globalization
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

		/// <summary>Gets an enumeration of translation options for edit mode.</summary>
		/// <param name="item">The item beeing edited.</param>
		/// <param name="includeCurrent">Wether to include the current item in the enumeratin.</param>
		/// <returns>An enumeration of translation definitions.</returns>
		IEnumerable<TranslateSpecification> GetEditTranslations(ContentItem item, bool includeCurrent);

		/// <summary>Gets available languages.</summary>
		/// <returns>An enumeration of available languages.</returns>
		IEnumerable<ILanguage> GetAvailableLanguages();
		
		/// <summary>Gets an item's translation in a certain language.</summary>
		/// <param name="item">The item whose translation to get.</param>
		/// <param name="language">The language to look for.</param>
		/// <returns>A translated item or null.</returns>
		ContentItem GetTranslation(ContentItem item, ILanguage language);
	}
}
