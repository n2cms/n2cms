using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Globalization
{
	public interface ILanguageGateway
	{
		IEnumerable<ContentItem> FindTranslations(ContentItem item);
		ILanguage FindLanguage(ContentItem item);

		IEnumerable<TranslationOption> GetTranslationOptions(ContentItem item, bool includeCurrent);
		IEnumerable<ILanguage> GetLanguages();
		
		ContentItem GetTranslation(ContentItem destination, ILanguage language);
	}
}
