using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Globalization
{
	public interface ILanguageGateway
	{
		IEnumerable<ILanguage> GetLanguages();
		IEnumerable<TranslationOption> GetTranslationOptions(ContentItem item, bool includeCurrent);
		IEnumerable<ContentItem> FindTranslations(ContentItem item);
	}
}
