using N2.Engine.Globalization;
using N2.Web.UI;

namespace N2.Templates.Mvc.Web
{
	public class LanguageModifier : IPageModifier
	{
		ILanguageGateway gateway;

		public LanguageModifier(ILanguageGateway gateway)
		{
			this.gateway = gateway;
		}

		public void Modify<T>(ContentPage<T> page) where T : ContentItem
		{
			ILanguage language = gateway.GetLanguage(page.CurrentPage);
			if (language != null && !string.IsNullOrEmpty(language.LanguageCode))
			{
				page.Culture = language.LanguageCode;
				page.UICulture = language.LanguageCode;
			}
		}
	}
}