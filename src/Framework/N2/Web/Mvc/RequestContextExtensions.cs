using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
using N2.Definitions;

namespace N2.Web.Mvc
{
    public static class RequestContextExtensions
    {
        public static T CurrentPart<T>(this RequestContext context) where T : ContentItem
        {
            var item = context.RouteData.CurrentPath().CurrentItem;
            if (item == null || item.IsPage)
                return null;
            return item as T;
        }

        public static ContentItem CurrentItem(this RequestContext context)
        {
            return context.RouteData.CurrentPath().CurrentItem;
        }
        public static T CurrentItem<T>(this RequestContext context) where T : ContentItem
        {
            return context.RouteData.CurrentPath().CurrentItem as T;
        }

        public static T CurrentPage<T>(this RequestContext context) where T : ContentItem
        {
            return context.RouteData.CurrentPath().CurrentPage as T;
        }
        public static ContentItem CurrentPage(this RequestContext context)
        {
            return context.RouteData.CurrentPath().CurrentPage;
        }

        public static ContentItem StartPage(this RequestContext context)
        {
            return Find.ClosestOf<IStartPage>(context.RouteData.CurrentPath().CurrentItem)
                ?? RouteExtensions.GetEngine(context.RouteData).Content.Traverse.StartPage;
        }
    }
}
