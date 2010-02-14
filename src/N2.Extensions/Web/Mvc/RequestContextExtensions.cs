using System;
using System.Web.Routing;

namespace N2.Web.Mvc
{
	public static class RequestContextExtensions
	{
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
			return context.CurrentItem<T>(ContentRoute.ContentItemKey);
		}
		public static T CurrentPage<T>(this RequestContext context) where T : ContentItem
		{
			return context.CurrentItem<T>(ContentRoute.ContentPageKey);
		}

		public static T CurrentItem<T>(this RequestContext context, string key) where T : ContentItem
		{
			if (context == null) throw new ArgumentNullException("context");

			return context.RouteData.DataTokens[key] as T;
		}
	}
}
