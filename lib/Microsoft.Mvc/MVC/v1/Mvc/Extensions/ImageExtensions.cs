using System.Web.Routing;
namespace System.Web.Mvc {
    public static class ImageExtensions {
        /// <summary>
        /// A Simple image
        /// </summary>
        /// <param name="imageRelativeUrl">The url for the image</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "1#", Justification = "The return value is not a regular URL since it may contain ~/ ASP.NET-specific characters")]
        public static string Image(this HtmlHelper helper, string imageRelativeUrl) {
            return Image(helper, imageRelativeUrl, null);
        }

        /// <summary>
        /// A Simple image
        /// </summary>
        /// <param name="imageRelativeUrl">The url for the image</param>
        /// <param name="alt">An alternate representation for the image for accessibility</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "1#", Justification = "Required for Extension Method")]
        public static string Image(this HtmlHelper helper, string imageRelativeUrl, string alt) {
            return Image(helper, imageRelativeUrl, alt, null);
        }

        /// <summary>
        /// A Simple image
        /// </summary>
        /// <param name="imageRelativeUrl">The url for the image</param>
        /// <param name="htmlName">ID of the Image (for use with javascript)</param>
        /// <param name="htmlAttributes">Any attributes you want set on the tag. Use anonymous-type declaration for this: new{class=cssclass}</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "1#", Justification = "The return value is not a regular URL since it may contain ~/ ASP.NET-specific characters")]
        public static string Image(this HtmlHelper helper, string imageRelativeUrl, object htmlAttributes) {
            return Image(helper, imageRelativeUrl, "", htmlAttributes);
        }

        /// <summary>
        /// A Simple image
        /// </summary>
        /// <param name="imageRelativeUrl">The url for the image</param>
        /// <param name="htmlName">ID of the Image (for use with javascript)</param>
        /// <param name="alt">An alternate representation for the image for accessibility</param>
        /// <param name="htmlAttributes">Any attributes you want set on the tag. Use anonymous-type declaration for this: new{class=cssclass}</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "1#", Justification = "The return value is not a regular URL since it may contain ~/ ASP.NET-specific characters")]
        public static string Image(this HtmlHelper helper, string imageRelativeUrl, string alt, object htmlAttributes) {
            string resolvedUrl = LinkExtensions.ResolveUrl(helper, imageRelativeUrl);
            return ImageBuilder.ImageTag(resolvedUrl, alt, new RouteValueDictionary(htmlAttributes));
        }
    }
}
