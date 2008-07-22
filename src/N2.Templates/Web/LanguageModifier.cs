using System;
using System.Collections.Generic;
using System.Text;

using N2.Templates.Web.UI;
using N2.Templates.Items;
using N2.Engine.Globalization;

namespace N2.Templates.Web
{
	public class LanguageModifier : IPageModifier
	{
		ILanguageGateway gateway;

		public LanguageModifier(ILanguageGateway gateway)
		{
			this.gateway = gateway;
		}

		public void Modify<T>(TemplatePage<T> page) 
			where T : AbstractPage
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
