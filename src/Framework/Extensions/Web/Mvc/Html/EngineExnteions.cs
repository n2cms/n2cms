using System.Linq;
using System.Web.Mvc;
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

		public static T ResolveService<T>(this HtmlHelper helper) where T : class
		{
			return helper.ViewContext.RouteData.ResolveService<T>();
		}

		public static T[] ResolveServices<T>(this HtmlHelper helper) where T : class
		{
			return helper.ViewContext.RouteData.ResolveServices<T>();
		}

		public static ContentItem RootPage(this HtmlHelper html)
		{
			return Find.EnumerateParents(html.CurrentItem(), null, true).LastOrDefault();
		}


		public static T ResolveAdapter<T>(this IEngine engine, ContentItem item) where T : AbstractContentAdapter
		{
			return engine.Resolve<IContentAdapterProvider>().ResolveAdapter<T>(item);
		}
	}
}
