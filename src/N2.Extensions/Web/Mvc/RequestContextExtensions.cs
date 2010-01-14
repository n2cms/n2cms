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
		public static T CurrentItem<T>(this RequestContext context) where T : ContentItem
		{
			if (context == null) throw new ArgumentNullException("context");

			return context.RouteData.DataTokens[ContentRoute.ContentItemKey] as T;
		}
		public static T CurrentPage<T>(this RequestContext context) where T : ContentItem
		{
			if (context == null) throw new ArgumentNullException("context");

			return context.RouteData.DataTokens[ContentRoute.ContentPageKey] as T;
		}
	}
}
