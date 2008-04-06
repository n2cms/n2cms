namespace System.Web.Mvc {
    using System;
    using System.Collections.Generic;
    using System.Web.Routing;

    public static class ImageBuilder {
        /// <summary>
        /// Creates an HTML IMG tag
        /// </summary>
        /// <param name="htmlName">The name of the control</param>
        /// <param name="sourceUrl">Tee SRC Url</param>
        /// <param name="htmlAttributes">Dictionary of settings</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "0#", Justification = "The return value is not a regular URL since it may contain ~/ ASP.NET-specific characters")]
        public static string ImageTag(string sourceUrl, string alt, RouteValueDictionary htmlAttributes) {

            htmlAttributes = htmlAttributes ?? new RouteValueDictionary();
            htmlAttributes.Add("src", sourceUrl);

            if (String.IsNullOrEmpty(alt))
                alt = System.IO.Path.GetFileName(sourceUrl);

            htmlAttributes.Add("alt", alt);
            string imageTag = TagBuilder.CreateTag(HtmlTagType.Image, "", htmlAttributes);
            return imageTag;

        }
    }
}
