using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using N2.Definitions;
using N2.Details;
using N2.Web.Mvc;
using N2.Web.Mvc.Html;
using System.Web.Hosting;

namespace N2.Web.Mvc.Html
{
	public static class ThemeExtensions
	{
		public static string ThemedContent(this UrlHelper url, string contentPath)
		{
			return ResolveThemedContent(url.RequestContext.HttpContext, HostingEnvironment.VirtualPathProvider, contentPath);
		}

		public static string ThemedStyleSheet(this HtmlHelper html, string stylePath)
		{
			return N2.Resources.Register.StyleSheet(html.ViewContext.ViewData, ResolveThemedContent(html.ViewContext.HttpContext, HostingEnvironment.VirtualPathProvider, stylePath));
		}

		private static string ResolveThemedContent(HttpContextBase httpContext, VirtualPathProvider vpp, string contentPath)
		{
			string theme = httpContext.GetTheme();
			if (string.IsNullOrEmpty(theme))
				return Url.ToAbsolute(contentPath);

			string themeContentPath = "~/Themes/" + theme + contentPath.TrimStart('~');
			if (!vpp.FileExists(themeContentPath))
				return Url.ToAbsolute(contentPath);

			return Url.ToAbsolute(themeContentPath);
		}

		// theming

		private const string ThemeKey = "theme";
		public static string GetTheme(this ControllerContext context)
		{
			return context.HttpContext.GetTheme();
		}

		internal static string GetTheme(this HttpContextBase context)
		{
			return context.Request[ThemeKey] // preview theme via query string
				?? context.Items[ThemeKey] as string // previously initialized theme
				?? "Default"; // fallback
		}

		public static void SetTheme(this ControllerContext context, string theme)
		{
			context.HttpContext.Items[ThemeKey] = theme;
		}

		public static void InitTheme(this ControllerContext context)
		{
			var page = context.RequestContext.CurrentPage<ContentItem>()
				?? RouteExtensions.ResolveService<IUrlParser>(context.RouteData).Parse(context.HttpContext.Request["returnUrl"])
				?? context.RequestContext.StartPage();

			InitTheme(context, page);
		}

		private static void InitTheme(ControllerContext context, ContentItem page)
		{
			var start = Find.ClosestOf<IThemeable>(page);
			if (start == null)
				return;

			var themeSource = start as IThemeable;
			if (string.IsNullOrEmpty(themeSource.Theme))
				InitTheme(context, start.Parent);
			else
				context.SetTheme(themeSource.Theme);
		}
	}
}