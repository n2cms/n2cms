using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using N2.Definitions;
using N2.Details;
using N2.Web.Mvc;
using N2.Web.Mvc.Html;
using System.Web.Hosting;
using System.Web.Routing;

namespace N2.Web.Mvc.Html
{
    public static class ThemeExtensions
    {
        public static string ThemedContent(this UrlHelper url, string contentPath)
        {
            return ResolveThemedContent(url.RequestContext, HostingEnvironment.VirtualPathProvider, contentPath);
        }

        public static string ThemedStyleSheet(this HtmlHelper html, string stylePath)
        {
            return N2.Resources.Register.StyleSheet(html.ViewContext.GetResourceStateCollection(), ResolveThemedContent(html.ViewContext.RequestContext, HostingEnvironment.VirtualPathProvider, stylePath));
        }

        private static string ResolveThemedContent(RequestContext requestContext, VirtualPathProvider vpp, string contentPath)
        {
            string themeFolderPath = requestContext.RouteData.DataTokens["ThemeViewEngine.ThemeFolderPath"] as string 
                ?? Url.ResolveTokens(Url.ThemesUrlToken);

            string theme = requestContext.HttpContext.GetTheme();
            if (!string.IsNullOrEmpty(theme))
            {
                string themeContentPath = themeFolderPath + theme + contentPath.TrimStart('~');
                if (vpp.FileExists(themeContentPath))
                    return Url.ToAbsolute(themeContentPath);
            }

            string defaultThemeContentPath = themeFolderPath + "Default" + contentPath.TrimStart('~');
            if (vpp.FileExists(defaultThemeContentPath))
                return Url.ToAbsolute(defaultThemeContentPath);

            return Url.ToAbsolute(contentPath);
        }
    }
}
