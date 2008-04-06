namespace System.Web.Mvc {
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Web.Routing;

    public static class ButtonsAndLinkExtensions {
        /// <summary>
        /// Creates a submit button for your form
        /// </summary>
        /// <param name="buttonText">The text for the button face</param>
        public static string SubmitButton(this HtmlHelper helper, string htmlName) {
            return SubmitButton(helper, htmlName, "Submit", null);
        }

        /// <summary>
        /// Creates a submit button for your form
        /// </summary>
        /// <param name="buttonText">The text for the button face</param>
        public static string SubmitButton(this HtmlHelper helper, string htmlName, string buttonText) {
            return SubmitButton(helper, htmlName, buttonText, null);
        }

        /// <summary>
        /// Creates a submit button for your form
        /// </summary>
        public static string SubmitButton(this HtmlHelper helper) {
            return SubmitButton(helper, "formSubmit", "Submit", null);
        }

        /// <summary>
        /// Creates a submit button for your form
        /// </summary>
        /// <param name="htmlName">Name of the button</param>
        /// <param name="buttonText">The text for the button face</param>
        /// <param name="htmlAttributes">Any attributes you want set on the tag. Use anonymous-type declaration for this: new{class=cssclass}</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "helper", Justification = "Required for Extension Method")]
        public static string SubmitButton(this HtmlHelper helper, string htmlName, string buttonText, object htmlAttributes) {
            return ButtonBuilder.SubmitButton(htmlName, buttonText, new RouteValueDictionary(htmlAttributes));
        }

        /// <summary>
        /// Creates a submit button for your form
        /// </summary>
        /// <param name="htmlName">Name of the button</param>
        /// <param name="buttonText">The text for the button face</param>
        /// <param name="htmlAttributes">Dictionary of HTML settings</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "helper", Justification = "Required for Extension Method")]
        public static string SubmitButton(this HtmlHelper helper, string htmlName, string buttonText, IDictionary<string, object> htmlAttributes) {
            return ButtonBuilder.SubmitButton(htmlName, buttonText, (htmlAttributes == null) ? new RouteValueDictionary() : new RouteValueDictionary(htmlAttributes));
        }

        /// <summary>
        /// Creates a submit button for your form using an image
        /// </summary>
        /// <param name="htmlName">Name of the button</param>
        /// <param name="buttonText">The text for the button face</param>
        /// <param name="htmlAttributes">Any attributes you want set on the tag. Use anonymous-type declaration for this: new{class=cssclass}</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "2#", Justification = "The return value is not a regular URL since it may contain ~/ ASP.NET-specific characters")]
        public static string SubmitImage(this HtmlHelper helper, string htmlName, string imageRelativeUrl) {
            return SubmitImage(helper, htmlName, imageRelativeUrl, null);
        }

        /// <summary>
        /// Creates a submit button for your form using an image
        /// </summary>
        /// <param name="htmlName">Name of the button</param>
        /// <param name="buttonText">The text for the button face</param>
        /// <param name="htmlAttributes">Any attributes you want set on the tag. Use anonymous-type declaration for this: new{class=cssclass}</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "2#", Justification = "The return value is not a regular URL since it may contain ~/ ASP.NET-specific characters")]
        public static string SubmitImage(this HtmlHelper helper, string htmlName, string imageRelativeUrl, object htmlAttributes) {
            string resolvedUrl = LinkExtensions.ResolveUrl(helper, imageRelativeUrl);
            return ButtonBuilder.SubmitImage(htmlName, resolvedUrl, new RouteValueDictionary(htmlAttributes));
        }

        /// <summary>
        /// Creates a submit button for your form using an image
        /// </summary>
        /// <param name="htmlName">Name of the button</param>
        /// <param name="buttonText">The text for the button face</param>
        /// <param name="htmlAttributes">Dictionary of HTML settings</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "2#", Justification = "The return value is not a regular URL since it may contain ~/ ASP.NET-specific characters")]
        public static string SubmitImage(this HtmlHelper helper, string htmlName, string imageRelativeUrl, IDictionary<string, object> htmlAttributes) {
            string resolvedUrl = LinkExtensions.ResolveUrl(helper, imageRelativeUrl);
            return ButtonBuilder.SubmitImage(htmlName, resolvedUrl, (htmlAttributes == null) ? new RouteValueDictionary() : new RouteValueDictionary(htmlAttributes));
        }

        /// <summary>
        /// A Simple button you can use with javascript
        /// </summary>
        /// <param name="htmlName">Name of the button</param>
        /// <param name="buttonText">The text for the button face</param>
        /// <param name="onClickMethod">The method or script routine to call when the button is clicked.</param>
        /// <param name="htmlAttributes">Any attributes you want set on the tag. Use anonymous-type declaration for this: new{class=cssclass}</param>
        /// <returns></returns>
        public static string Button(this HtmlHelper helper, string htmlName, string buttonText, string onClickMethod) {
            return Button(helper, htmlName, buttonText, onClickMethod, null);
        }

        /// <summary>
        /// A Simple button you can use with javascript
        /// </summary>
        /// <param name="htmlName">Name of the button</param>
        /// <param name="buttonText">The text for the button face</param>
        /// <param name="onClickMethod">The method or script routine to call when the button is clicked.</param>
        /// <param name="htmlAttributes">Any attributes you want set on the tag. Use anonymous-type declaration for this: new{class=cssclass}</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "helper", Justification = "This is an Extension Method and requires this argument")]
        public static string Button(this HtmlHelper helper, string htmlName, string buttonText, string onClickMethod, object htmlAttributes) {
            string result = ButtonBuilder.Button(htmlName, buttonText, onClickMethod, new RouteValueDictionary(htmlAttributes));
            return result;
        }

        /// <summary>
        /// A Simple button you can use with javascript
        /// </summary>
        /// <param name="htmlName">Name of the button</param>
        /// <param name="buttonText">The text for the button face</param>
        /// <param name="onClickMethod">The method or script routine to call when the button is clicked.</param>
        /// <param name="htmlAttributes">Any attributes you want set on the tag. Use anonymous-type declaration for this: new{class=cssclass}</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "helper", Justification = "This is an Extension Method and requires this argument")]
        public static string Button(this HtmlHelper helper, string htmlName, string buttonText, string onClickMethod, IDictionary<string, object> htmlAttributes) {
            string result = ButtonBuilder.Button(htmlName, buttonText, onClickMethod, (htmlAttributes == null) ? new RouteValueDictionary() : new RouteValueDictionary(htmlAttributes));
            return result;
        }

        /// <summary>
        /// A Simple button you can use with javascript
        /// </summary>
        /// <param name="htmlName">Name of the button</param>
        /// <param name="buttonText">The text for the button face</param>
        /// <param name="onClickMethod">The method or script routine to call when the button is clicked.</param>
        /// <param name="htmlAttributes">Any attributes you want set on the tag. Use anonymous-type declaration for this: new{class=cssclass}</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an Extension Method which allows the user to provide a strongly-typed argument via Expression")]
        public static string Button<T>(this HtmlHelper helper, Expression<Action<T>> action, string htmlName, string buttonText) where T : Controller {
            string link = LinkExtensions.BuildUrlFromExpression<T>(helper, action);
            return Button(helper, htmlName, buttonText, "location.href='" + link + "'", null);
        }

        /// <summary>
        /// A Simple button you can use with javascript
        /// </summary>
        /// <param name="htmlName">Name of the button</param>
        /// <param name="buttonText">The text for the button face</param>
        /// <param name="onClickMethod">The method or script routine to call when the button is clicked.</param>
        /// <param name="htmlAttributes">Any attributes you want set on the tag. Use anonymous-type declaration for this: new{class=cssclass}</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an Extension Method which allows the user to provide a strongly-typed argument via Expression")]
        public static string Button<T>(this HtmlHelper helper, Expression<Action<T>> action, string htmlName, string buttonText, object htmlAttributes) where T : Controller {
            string link = LinkExtensions.BuildUrlFromExpression<T>(helper, action);
            return Button(helper, htmlName, buttonText, "location.href='" + link + "'", htmlAttributes);
        }

        /// <summary>
        /// A Simple button you can use with javascript
        /// </summary>
        /// <param name="htmlName">Name of the button</param>
        /// <param name="buttonText">The text for the button face</param>
        /// <param name="onClickMethod">The method or script routine to call when the button is clicked.</param>
        /// <param name="htmlAttributes">Dictionary settings for HTML Attributes</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an Extension Method which allows the user to provide a strongly-typed argument via Expression")]
        public static string Button<T>(this HtmlHelper helper, Expression<Action<T>> action, string htmlName, string buttonText, IDictionary<string, object> htmlAttributes) where T : Controller {
            string link = LinkExtensions.BuildUrlFromExpression<T>(helper, action);
            return Button(helper, htmlName, buttonText, "location.href='" + link + "'", (htmlAttributes == null) ? new RouteValueDictionary() : new RouteValueDictionary(htmlAttributes));
        }

        /// <summary>
        /// A Simple button you can use with javascript
        /// </summary>
        /// <param name="htmlName">Name of the button</param>
        /// <param name="buttonText">The text for the button face</param>
        /// <param name="navigateToUrl">The URL to navigate to onCLick.</param>
        /// <returns></returns>
        public static string NavigateButton(this HtmlHelper helper, string htmlName, string buttonText, string navigateToUrl) {
            return NavigateButton(helper, htmlName, buttonText, navigateToUrl, null);
        }

        /// <summary>
        /// A Simple button you can use with javascript
        /// </summary>
        /// <param name="htmlName">Name of the button</param>
        /// <param name="buttonText">The text for the button face</param>
        /// <param name="navigateToUrl">The URL to navigate to onCLick.</param>
        /// <param name="htmlAttributes">Any attributes you want set on the tag. Use anonymous-type declaration for this: new{class=cssclass}</param>
        /// <returns></returns>
        public static string NavigateButton(this HtmlHelper helper, string htmlName, string buttonText, string navigateToUrl, object htmlAttributes) {
            return Button(helper, htmlName, buttonText, "location.href='" + helper.Encode(navigateToUrl) + "'", htmlAttributes);
        }

        /// <summary>
        /// A Simple button you can use with javascript
        /// </summary>
        /// <param name="htmlName">Name of the button</param>
        /// <param name="buttonText">The text for the button face</param>
        /// <param name="navigateToUrl">The URL to navigate to onCLick.</param>
        /// <param name="htmlAttributes">Dictionary settings for HTML Attributes</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "3#")]
        public static string NavigateButton(this HtmlHelper helper, string htmlName, string buttonText, string navigateToUrl, IDictionary<string, object> htmlAttributes) {
            return Button(helper, htmlName, buttonText, "location.href='" + helper.Encode(navigateToUrl) + "'", (htmlAttributes == null) ? new RouteValueDictionary() : new RouteValueDictionary(htmlAttributes));
        }
    }
}
