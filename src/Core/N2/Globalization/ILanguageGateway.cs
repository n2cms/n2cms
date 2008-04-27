using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Globalization
{
	public interface ILanguageGateway
	{
		IEnumerable<TranslationOption> GetTranslationOptions(ContentItem item, bool includeCurrent);
		IEnumerable<ContentItem> FindTranslations(ContentItem item);

		IEnumerable<ILanguage> GetLanguages();
		ILanguage GetLanguageAncestor(ContentItem item);

		ContentItem GetTranslation(ContentItem destination, ILanguage language);
	}
}
