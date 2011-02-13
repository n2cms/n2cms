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
			string theme = url.RequestContext.HttpContext.GetTheme();
			if (string.IsNullOrEmpty(theme))
				return url.Content(contentPath);

			string themeContentPath = "~/Themes/" + theme + contentPath.TrimStart('~');
			if (!HostingEnvironment.VirtualPathProvider.FileExists(themeContentPath))
				return url.Content(contentPath);

			return url.Content(themeContentPath);
		}

		private const string ThemeKey = "theme";
		public static string GetTheme(this ControllerContext context)
		{
			return context.HttpContext.GetTheme();
		}

		internal static string GetTheme(this HttpContextBase context)
		{
			return context.Request[ThemeKey] 
				?? context.Items[ThemeKey] as string 
				?? "Default";
		}

		public static void SetTheme(this ControllerContext context, string theme)
		{
			context.HttpContext.Items[ThemeKey] = theme;
		}

		public static void InitTheme(this ControllerContext context)
		{
			var page = context.RequestContext.CurrentPage<ContentItem>() ?? N2.Find.StartPage;

			var start = Find.Closest<IThemeable>(page);
			if (start == null)
				return;

			context.SetTheme(start.Theme);
		}
	}
}