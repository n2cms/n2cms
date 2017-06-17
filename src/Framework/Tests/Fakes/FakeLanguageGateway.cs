using System;
using System.Collections.Generic;
using N2.Engine.Globalization;

namespace N2.Tests.Fakes
{
	public class FakeLanguageGateway : ILanguageGateway
	{
		public FakeLanguageGateway()
		{
		}

		public bool Enabled
		{
			get
			{
				return false;
			}
		}

		public void Associate(IEnumerable<ContentItem> items)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<ContentItem> FindTranslations(ContentItem item)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<ILanguage> GetAvailableLanguages()
		{
			throw new NotImplementedException();
		}

		public IEnumerable<TranslateSpecification> GetEditTranslations(ContentItem item, bool includeCurrent, bool generateNonTranslated)
		{
			throw new NotImplementedException();
		}

		public ILanguage GetLanguage(string languageCode)
		{
			throw new NotImplementedException();
		}

		public ILanguage GetLanguage(ContentItem item)
		{
			throw new NotImplementedException();
		}

		public ContentItem GetTranslation(ContentItem item, ILanguage language)
		{
			throw new NotImplementedException();
		}

		public bool IsLanguageRoot(ContentItem item)
		{
			return item["IsRoot"] as bool? ?? false;
		}

		public void Unassociate(ContentItem item)
		{
			throw new NotImplementedException();
		}
	}
}