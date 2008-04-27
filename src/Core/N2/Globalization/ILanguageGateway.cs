using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Globalization
{
	public interface ILanguageGateway
	{
		IEnumerable<ILanguageRoot> GetLanguages();
		IEnumerable<TranslationOption> GetTranslationOptions(ContentItem item);
		IEnumerable<ContentItem> FindTranslations(ContentItem item);
	}
}
