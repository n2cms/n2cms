using System;
using System.Linq;
using System.Web.Routing;
using N2.Persistence;
using N2.Engine;
using System.Collections.Generic;

namespace N2.Web.Mvc
{
	public static class RequestContextExtensions
	{
		public static string ToQueryString<K,V>(this IDictionary<K,V> values)
		{
			if (values == null)
				return null;
			return string.Join("&", values.Select(kvp => kvp.Key + "=" + kvp.Value).ToArray());
		}

		public static ContentItem CurrentItem(this RequestContext context)
		{
			return context.CurrentItem<ContentItem>();
		}
		public static T CurrentPart<T>(this RequestContext context) where T : ContentItem
		{
			return context.CurrentItem<T>(ContentRoute.ContentPartKey);
		}
		public static T CurrentItem<T>(this RequestContext context) where T : ContentItem
		{
			return context.CurrentItem<T>(ContentRoute.ContentItemKey)
				?? context.CurrentItem<T>(ContentRoute.ContentPartKey)
				?? context.CurrentItem<T>(ContentRoute.ContentPageKey);
		}
		public static T CurrentPage<T>(this RequestContext context) where T : ContentItem
		{
			return context.CurrentItem<T>(ContentRoute.ContentPageKey);
		}

		internal static T CurrentItem<T>(this RequestContext context, string key) where T : ContentItem
		{
			if (context == null) throw new ArgumentNullException("context");

			return context.RouteData.DataTokens[key] as T
				?? context.RouteData.Values.CurrentItem<T>(key, context.RouteData.GetEngine().Persister);
		}

		private static IEngine GetEngine(this RouteData routeData)
		{
			return routeData.DataTokens[ContentRoute.ContentEngineKey] as IEngine
				?? N2.Context.Current;
		}
	}
}
