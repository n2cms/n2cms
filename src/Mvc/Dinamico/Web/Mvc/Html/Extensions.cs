using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using N2.Definitions;
using N2.Details;
using System.Web.Hosting;

namespace N2.Web.Mvc.Html
{
	public static class Extensions
	{
		public static IHtmlString ToHtmlString(this object instance)
		{
			if (instance == null)
				return null;
			return new HtmlString(instance.ToString());
		}

		public static string ThemedContent(this UrlHelper url, string contentPath)
		{
			string theme = url.RequestContext.RouteData.DataTokens[HandleThemeAttribute.ThemeKey] as string;
			if (string.IsNullOrEmpty(theme))
				return url.Content(contentPath);

			string themeContentPath = "~/Themes/" + theme + contentPath.TrimStart('~');
			if (!HostingEnvironment.VirtualPathProvider.FileExists(themeContentPath))
				return url.Content(contentPath);

			return url.Content(themeContentPath);
		}
	}
}