using N2.Definitions.Runtime;
using N2.Details;
using System.Web;
using System.Web.Mvc;

namespace N2.Web.Mvc.Html
{
    public static class RegistrationExtensions
    {
        const string RegistrationExpressionKey = "RegistrationExpression";

		public static bool IsRegistering(this HtmlHelper html)
		{
			return GetRegistrationExpression(html) != null;
		}

        public static ContentRegistration GetRegistrationExpression(HtmlHelper html)
        {
            return GetRegistrationExpression(html.ViewContext.HttpContext);
        }
        public static ContentRegistration GetRegistrationExpression(HttpContextBase httpContext)
        {
            return httpContext.Items[RegistrationExpressionKey] as ContentRegistration;
        }

        public static void SetRegistrationExpression(HttpContextBase httpContext, ContentRegistration re)
        {
            httpContext.Items[RegistrationExpressionKey] = re;
        }

        public static IContentRegistration DefaultValue(this IContentRegistration registration, string name, object value)
        {
            if (registration == null) return null;

            registration.RegisterModifier(new DefaultValueAttribute { Name = name, Value = value });
            return registration;
        }
    }
}
