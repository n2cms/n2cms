using System.Web.Mvc;
using N2.Definitions;
using N2.Engine;

namespace N2.Web.Mvc.Html
{
	public static class EngineExnteions
	{
		public static IEngine ContentEngine(this HtmlHelper html)
		{
			return html.ViewContext.RouteData.DataTokens[ContentRoute.ContentEngineKey] as IEngine
				?? N2.Context.Current;
		}

		public static ItemDefinition Definition(this HtmlHelper html, ContentItem item)
		{
			return html.ContentEngine().Definitions.GetDefinition(item);
		}

		public static T ResolveService<T>(this HtmlHelper helper) where T : class
		{
			return RouteExtensions.ResolveService<T>(helper.ViewContext.RouteData);
		}

		public static T[] ResolveServices<T>(this HtmlHelper helper) where T : class
		{
			return RouteExtensions.ResolveServices<T>(helper.ViewContext.RouteData);
		}

		public static T ResolveAdapter<T>(this IEngine engine, ContentItem item) where T : AbstractContentAdapter
		{
			return engine.Resolve<IContentAdapterProvider>().ResolveAdapter<T>(item);
		}
	}
}
