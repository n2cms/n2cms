namespace System.Web.Mvc {
    using System;
    using System.Web.Routing;

    public static class ButtonBuilder {
        /// <summary>
        /// Creates a form Submit Button
        /// </summary>
        /// <param name="htmlName">Name of the button</param>
        /// <param name="buttonText">Text for the button</param>
        /// <param name="htmlAttributes">Dictionary of settings</param>
        /// <returns>System.String</returns>
        public static string SubmitButton(string htmlName, string buttonText, RouteValueDictionary htmlAttributes) {
            htmlAttributes = htmlAttributes ?? new RouteValueDictionary();
            htmlAttributes.Add("value", buttonText);
            return TagBuilder.CreateInputTag(HtmlInputType.Submit, htmlName, htmlAttributes);
        }

        /// <summary>
        /// Creates a form Submit Image
        /// </summary>
        /// <param name="htmlName">Name of the button</param>
        /// <param name="sourcUrl">The URL of the image to use (relative)</param>
        /// <param name="htmlAttributes">Dictionary of settings</param>
        /// <returns>System.String</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "1#", Justification = "The return value is not a regular URL since it may contain ~/ ASP.NET-specific characters")]
        public static string SubmitImage(string htmlName, string sourceUrl, RouteValueDictionary htmlAttributes) {
            htmlAttributes = htmlAttributes ?? new RouteValueDictionary();
            htmlAttributes.Add("src", sourceUrl);
            string result = TagBuilder.CreateInputTag(HtmlInputType.Image, htmlName, htmlAttributes);
            return result;
        }

        /// <summary>
        /// Creates a Button
        /// </summary>
        /// <param name="htmlName">Name of the button</param>
        /// <param name="buttonText">Text for the button</param>
        /// <param name="onClickMethod">The javascript method to call when the button is clicked</param>
        /// <param name="htmlAttributes">Dictionary of settings</param>
        /// <returns>System.String</returns>
        public static string Button(string htmlName, string buttonText, string onClickMethod, RouteValueDictionary htmlAttributes) {
            htmlAttributes = htmlAttributes ?? new RouteValueDictionary();

            if (!String.IsNullOrEmpty(onClickMethod))
                htmlAttributes.Add("onclick", onClickMethod);

            htmlAttributes.Add("value", buttonText);

            string result = TagBuilder.CreateTag(HtmlTagType.Button, htmlName, htmlAttributes);
            return result;
        }
    }
}
