using System.Web.UI;
using N2.Engine.Globalization;
using N2.Web;

namespace N2.Templates.Mvc.Web
{
	public class LanguageModifier : IPageModifier
	{
		readonly ILanguageGateway gateway;
		private readonly IUrlParser urlParser;

		public LanguageModifier(ILanguageGateway gateway, IUrlParser urlParser)
		{
			this.gateway = gateway;
			this.urlParser = urlParser;
		}

		public void Modify(Page page)
		{
			ILanguage language = gateway.GetLanguage(urlParser.CurrentPage);
			if (language == null || string.IsNullOrEmpty(language.LanguageCode)) 
				return;

			page.Culture = language.LanguageCode;
			page.UICulture = language.LanguageCode;
		}
	}
}