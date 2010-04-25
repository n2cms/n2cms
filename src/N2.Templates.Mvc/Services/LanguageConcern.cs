using System.Web.UI;
using N2.Engine.Globalization;
using N2.Web;
using N2.Templates.Mvc.Services;
using N2.Engine;

namespace N2.Templates.Mvc.Web
{
	[Service(typeof(IViewConcern))]
	public class LanguageConcern : IViewConcern
	{
		readonly ILanguageGateway gateway;

		public LanguageConcern(ILanguageGateway gateway)
		{
			this.gateway = gateway;
		}

		#region IViewConcern Members

		public void Apply(ContentItem item, Page page)
		{
			ILanguage language = gateway.GetLanguage(item);
			if (language == null || string.IsNullOrEmpty(language.LanguageCode)) 
				return;

			page.Culture = language.LanguageCode;
			page.UICulture = language.LanguageCode;
		}

		#endregion
	}
}