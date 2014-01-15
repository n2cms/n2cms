using System;
using System.Linq;
using System.Web.Mvc;
using N2.Definitions;

namespace N2.Web.Mvc.Html
{
    public static class ContentItemExtensions
    {
        public static PathData CurrentPath(this HtmlHelper helper)
        {
            if (helper == null) throw new ArgumentNullException("helper");

            return helper.ViewContext.RouteData.CurrentPath();
        }

        public static ContentItem CurrentPage(this HtmlHelper helper)
        {
            return CurrentPath(helper).CurrentPage;
        }
        public static T CurrentPage<T>(this HtmlHelper helper) where T : ContentItem
        {
            return CurrentPath(helper).CurrentPage as T;
        }

        public static ContentItem CurrentItem(this HtmlHelper helper)
        {
            return CurrentPath(helper).CurrentItem;
        }
        public static T CurrentItem<T>(this HtmlHelper helper) where T : ContentItem
        {
            return CurrentPath(helper).CurrentItem as T;
        }


        [Obsolete]
        public static ContentItem CurrentItem(this ViewContext context)
        {
            return context.CurrentItem<ContentItem>();
        }
        [Obsolete]
        public static T CurrentItem<T>(this ViewContext context) where T : ContentItem
        {
            if (context == null) throw new ArgumentNullException("context");
            
            return context.ViewData.CurrentItem<T>()
                ?? context.RequestContext.CurrentItem<T>();
        }
        [Obsolete]
        public static T CurrentPage<T>(this ViewContext context) where T : ContentItem
        {
            if (context == null) throw new ArgumentNullException("context");

            return context.ViewData.CurrentPage<T>()
                ?? context.RequestContext.CurrentPage<T>();
        }
        [Obsolete]
        private static T CurrentItem<T>(this ViewDataDictionary viewData) where T : ContentItem
        {
            if (viewData == null) throw new ArgumentNullException("viewData");

            return viewData[ContentRoute.ContentItemKey] as T
                ?? viewData.Model as T;
        }
        [Obsolete]
        private static T CurrentPage<T>(this ViewDataDictionary viewData) where T : ContentItem
        {
            if (viewData == null) throw new ArgumentNullException("viewData");

            return viewData[ContentRoute.ContentPageKey] as T;
        }



        public static ContentItem StartPage(this HtmlHelper html)
        {
            return html.ViewContext.RequestContext.StartPage();
        }

        public static T StartPage<T>(this HtmlHelper html) where T : ContentItem
        {
            return Find.Closest<T>(CurrentPath(html).CurrentItem)
                ?? EngineExnteions.ContentEngine(html).Content.Traverse.StartPage as T;
        }

        public static ContentItem RootPage(this HtmlHelper html)
        {
            return Find.EnumerateParents(CurrentPath(html).CurrentItem, null, true).LastOrDefault()
                ?? EngineExnteions.ContentEngine(html).Content.Traverse.RootPage;
        }
    }
}
