using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Definitions.Runtime;
using System.Web.Mvc;

namespace N2.Web.Mvc.Html
{
	public static class RegistrationExtensions
	{
		public static ContentRegistration GetRegistrationExpression(HtmlHelper html)
		{
			return html.ViewContext.ViewData["RegistrationExpression"] as ContentRegistration;
		}
	}
}
