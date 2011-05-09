using System.Collections.Generic;
using System.Web.Mvc;
using N2.Definitions.Runtime;

namespace N2.Web.Mvc.Html
{
	public static class RegistrationExtensions
	{
		public static ContentRegistration GetRegistrationExpression(HtmlHelper html)
		{
			return GetRegistrationExpression(html.ViewContext.ViewData);
		}
		public static ContentRegistration GetRegistrationExpression(IDictionary<string, object> viewData)
		{
			return viewData["RegistrationExpression"] as ContentRegistration;
		}

		public static IContentRegistration DefaultValue(this IContentRegistration registration, string name, object value)
		{
			if (registration == null) return null;

			registration.RegisterModifier(new N2.Details.DefaultValueAttribute { Name = name, Value = value });
			return registration;
		}
	}
}
