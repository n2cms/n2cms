using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;

namespace N2.Web.Mvc.Html
{
    public static class ContentItemExtensions
    {
        public static ContentItem CurrentPage(this HtmlHelper helper)
        {
            return helper.CurrentPage<ContentItem>();
        }
        public static T CurrentPage<T>(this HtmlHelper helper) where T : ContentItem
        {
            if (helper == null) throw new ArgumentNullException("helper");

            return helper.ViewContext.CurrentItem<T>();
        }

        public static ContentItem CurrentItem(this HtmlHelper helper)
        {
            return helper.CurrentItem<ContentItem>();
        }
        public static T CurrentItem<T>(this HtmlHelper helper) where T : ContentItem
        {
            if (helper == null) throw new ArgumentNullException("helper");

            return helper.ViewContext.CurrentItem<T>();
        }



        public static ContentItem CurrentItem(this ViewContext context)
        {
            return context.CurrentItem<ContentItem>();
        }
        public static T CurrentItem<T>(this ViewContext context) where T : ContentItem
        {
            if (context == null) throw new ArgumentNullException("context");
            
            return context.ViewData.CurrentItem<T>()
                ?? context.RequestContext.CurrentItem<T>();
        }

        private static T CurrentItem<T>(this ViewDataDictionary viewData) where T : ContentItem
        {
            if (viewData == null) throw new ArgumentNullException("viewData");

            return viewData.Model as T
                ?? viewData[ContentRoute.ContentItemKey] as T;
        }


        public static ContentItem CurrentItem(this RequestContext context)
        {
            return context.CurrentItem<ContentItem>();
        }
        public static T CurrentItem<T>(this RequestContext context) where T:ContentItem
        {
            if (context == null) throw new ArgumentNullException("context");
            
            return context.RouteData.Values[ContentRoute.ContentItemKey] as T;
        }
    }
}
