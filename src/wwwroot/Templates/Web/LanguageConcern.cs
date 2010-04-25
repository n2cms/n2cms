using N2.Templates.Web.UI;
using N2.Templates.Items;
using N2.Engine.Globalization;
using N2.Web.UI;
using N2.Templates.Services;
using N2.Engine;

namespace N2.Templates.Web
{
	[Service(typeof(TemplateConcern))]
	public class LanguageConcern : TemplateConcern
	{
		ILanguageGateway gateway;

		public LanguageConcern(ILanguageGateway gateway)
		{
			this.gateway = gateway;
		}

		public override void OnPreInit(ITemplatePage template)
		{
			var page = template.Page;
			ILanguage language = gateway.GetLanguage(template.CurrentItem);
			if (language != null && !string.IsNullOrEmpty(language.LanguageCode))
			{
				page.Culture = language.LanguageCode;
				page.UICulture = language.LanguageCode;
			}
		}
	}
}
