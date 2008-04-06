namespace System.Web.Mvc {
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq.Expressions;
    using System.Web;
    using System.Web.Routing;

    public static class LinkExtensions {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings", Justification = "This is a UI method and is required to use strings as Uri"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an Extension Method which allows the user to provide a strongly-typed argument via Expression")]
        public static string BuildUrlFromExpression<T>(this HtmlHelper helper, Expression<Action<T>> action) where T : Controller {
            return LinkBuilder.BuildUrlFromExpression<T>(helper.ViewContext, action);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "This method is specifically for Expressions and requires T")]
        public static RouteValueDictionary BuildQueryStringFromExpression(MethodCallExpression call) {
            return LinkBuilder.BuildParameterValuesFromExpression(call);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings", Justification = "The return value is not a regular URL since it may contain ~/ ASP.NET-specific characters"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "1#", Justification = "The return value is not a regular URL since it may contain ~/ ASP.NET-specific characters")]
        internal static string ResolveUrl(HtmlHelper helper, string virtualUrl) {
            HttpContextBase ctx = helper.ViewContext.HttpContext;
            string result = virtualUrl;

            if (virtualUrl.StartsWith("~/", StringComparison.OrdinalIgnoreCase)) {
                virtualUrl = virtualUrl.Remove(0, 2);

                //get the site root
                string siteRoot = ctx.Request.ApplicationPath;

                if (String.IsNullOrEmpty(siteRoot))
                    siteRoot = "/";

                result = siteRoot + virtualUrl;
            }
            return result;
        }

        /// <summary>
        /// Creates an anchor tag based on the passed in controller type and method
        /// </summary>
        /// <typeparam name="T">The Controller Type</typeparam>
        /// <param name="action">The Method to route to</param>
        /// <param name="linkText">The linked text to appear on the page</param>
        /// <returns>System.String</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an Extension Method which allows the user to provide a strongly-typed argument via Expression")]
        public static string ActionLink<T>(this HtmlHelper helper, Expression<Action<T>> action, string linkText) where T : Controller {
            return ActionLink<T>(helper, action, linkText, null);
        }

        /// <summary>
        /// Creates an anchor tag based on the passed in controller type and method
        /// </summary>
        /// <typeparam name="T">The Controller Type</typeparam>
        /// <param name="action">The Method to route to</param>
        /// <param name="linkText">The linked text to appear on the page</param>
        /// <returns>System.String</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an Extension Method which allows the user to provide a strongly-typed argument via Expression")]
        public static string ActionLink<T>(this HtmlHelper helper, Expression<Action<T>> action, string linkText, object htmlAttributes) where T : Controller {

            //TODO: refactor this to work with ActionLink in the core
            string linkFormat = "<a href=\"{0}\" {1}>{2}</a>";
            string atts = string.Empty;

            if (htmlAttributes != null)
                atts = HtmlExtensionUtility.ConvertObjectToAttributeList(htmlAttributes);

            string link = BuildUrlFromExpression(helper, action);
            string result = string.Format(CultureInfo.InvariantCulture, linkFormat, link, atts, helper.Encode(linkText));
            return result;
        }

        /// <summary>
        /// Creates a MailTo link
        /// </summary>
        /// <param name="emailAddress">The address of the recipient. For multiple addresses, use a comma</param>
        /// <param name="linkText">The text of the MailTo link</param>
        /// <param name="subject">Subject of the email</param>
        /// <returns></returns>
        public static string Mailto(this HtmlHelper helper, string emailAddress, string linkText) {
            return Mailto(helper, emailAddress, linkText, "", "", "", "", null);
        }

        /// <summary>
        /// Creates a MailTo link
        /// </summary>
        /// <param name="emailAddress">The address of the recipient. For multiple addresses, use a comma</param>
        /// <param name="linkText">The text of the MailTo link</param>
        /// <param name="subject">Subject of the email</param>
        /// <returns></returns>
        public static string Mailto(this HtmlHelper helper, string emailAddress, string linkText, string subject) {
            return Mailto(helper, emailAddress, linkText, subject, "", "", "", null);
        }

        /// <summary>
        /// Creates a MailTo link
        /// </summary>
        /// <param name="emailAddress">The address of the recipient. For multiple addresses, use a comma</param>
        /// <param name="linkText">The text of the MailTo link</param>
        /// <param name="subject">Subject of the email</param>
        /// <param name="body">Text you want to appear in the body of the email</param>
        /// <returns></returns>
        public static string Mailto(this HtmlHelper helper, string emailAddress, string linkText, string subject, string body) {
            return Mailto(helper, emailAddress, linkText, subject, "", "", body, null);
        }

        /// <summary>
        /// Creates a MailTo link
        /// </summary>
        /// <param name="emailAddress">The address of the recipient. For multiple addresses, use a comma</param>
        /// <param name="linkText">The text of the MailTo link</param>
        /// <param name="subject">Subject of the email</param>
        /// <param name="body">Text you want to appear in the body of the email</param>
        /// <param name="cc">Copy-to email address</param>
        /// <param name="bcc">Blind copy address</param>
        /// <returns></returns>
        public static string Mailto(this HtmlHelper helper, string emailAddress, string linkText, string subject, string body, string cc, string bcc) {
            return Mailto(helper, emailAddress, linkText, subject, cc, bcc, body, null);
        }

        /// <summary>
        /// Creates a MailTo link
        /// </summary>
        /// <param name="emailAddress">The address of the recipient. For multiple addresses, use a comma</param>
        /// <param name="linkText">The text of the MailTo link</param>
        /// <param name="subject">Subject of the email</param>
        /// <returns></returns>
        /// <param name="htmlAttributes">Any extra HTML attributes you want to appear. Use Anonymous typing for this (new{_class=myclass})</param>
        public static string Mailto(this HtmlHelper helper, string emailAddress, string linkText, object htmlAttributes) {
            return Mailto(helper, emailAddress, linkText, "", "", "", "", htmlAttributes);
        }

        /// <summary>
        /// Creates a MailTo link
        /// </summary>
        /// <param name="emailAddress">The address of the recipient. For multiple addresses, use a comma</param>
        /// <param name="linkText">The text of the MailTo link</param>
        /// <param name="subject">Subject of the email</param>
        /// <returns></returns>
        /// <param name="htmlAttributes">Any extra HTML attributes you want to appear. Use Anonymous typing for this (new{_class=myclass})</param>
        public static string Mailto(this HtmlHelper helper, string emailAddress, string linkText, string subject, object htmlAttributes) {
            return Mailto(helper, emailAddress, linkText, subject, "", "", "", htmlAttributes);
        }

        /// <summary>
        /// Creates a MailTo link
        /// </summary>
        /// <param name="emailAddress">The address of the recipient. For multiple addresses, use a comma</param>
        /// <param name="linkText">The text of the MailTo link</param>
        /// <param name="subject">Subject of the email</param>
        /// <param name="body">Text you want to appear in the body of the email</param>
        /// <returns></returns>
        /// <param name="htmlAttributes">Any extra HTML attributes you want to appear. Use Anonymous typing for this (new{_class=myclass})</param>
        public static string Mailto(this HtmlHelper helper, string emailAddress, string linkText, string subject, string body, object htmlAttributes) {
            return Mailto(helper, emailAddress, linkText, subject, "", "", body, htmlAttributes);
        }

        /// <summary>
        /// Creates a MailTo link
        /// </summary>
        /// <param name="emailAddress">The address of the recipient. For multiple addresses, use a comma</param>
        /// <param name="linkText">The text of the MailTo link</param>
        /// <param name="subject">Subject of the email</param>
        /// <param name="body">Text you want to appear in the body of the email</param>
        /// <param name="cc">Copy-to email address</param>
        /// <param name="bcc">Blind copy address</param>
        /// <param name="htmlAttributes">Any extra HTML attributes you want to appear. Use Anonymous typing for this (new{_class=myclass})</param>
        /// <returns></returns>
        public static string Mailto(this HtmlHelper helper, string emailAddress, string linkText, string subject, string body, string cc, string bcc, object htmlAttributes) {
            return Mailto(helper, emailAddress, linkText, subject, body, cc, bcc, new RouteValueDictionary(htmlAttributes));
        }

        /// <summary>
        /// Creates a MailTo link
        /// </summary>
        /// <param name="emailAddress">The address of the recipient. For multiple addresses, use a comma</param>
        /// <param name="linkText">The text of the MailTo link</param>
        /// <param name="subject">Subject of the email</param>
        /// <param name="body">Text you want to appear in the body of the email</param>
        /// <param name="cc">Copy-to email address</param>
        /// <param name="bcc">Blind copy address</param>
        /// <param name="htmlAttributes">Dictionary of HTML Attributes</param>
        /// <returns></returns>
        public static string Mailto(this HtmlHelper helper, string emailAddress, string linkText, string subject,
            string body, string cc, string bcc, IDictionary<string, object> htmlAttributes) {

            string mailToUrl = "mailto:" + emailAddress;

            List<string> mailQuery = new List<string>();
            htmlAttributes = htmlAttributes ?? new RouteValueDictionary();

            if (!String.IsNullOrEmpty(subject))
                mailQuery.Add("subject=" + helper.Encode(subject));

            if (!String.IsNullOrEmpty(cc))
                mailQuery.Add("cc=" + helper.Encode(cc));

            if (!String.IsNullOrEmpty(bcc))
                mailQuery.Add("bcc=" + helper.Encode(bcc));

            if (!String.IsNullOrEmpty(body))
                mailQuery.Add("body=" + helper.Encode(body));

            int index = 0;
            foreach (string s in mailQuery) {
                if (index == 0)
                    mailToUrl += "?";
                else
                    mailToUrl += "&";

                mailToUrl += s;
                index++;
            }

            htmlAttributes.Add("href", mailToUrl);
            htmlAttributes.Add("value", linkText);
            string mailTag = TagBuilder.CreateTag(HtmlTagType.Mailto, "", new RouteValueDictionary(htmlAttributes));

            return mailTag;
        }
    }
}
