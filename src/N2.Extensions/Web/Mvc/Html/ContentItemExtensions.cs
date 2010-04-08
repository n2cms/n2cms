using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using N2.Engine;

namespace N2.Web.Mvc.Html
{
    public static class ContentItemExtensions
    {
		public static T ResolveService<T>(this HtmlHelper helper) where T: class
		{
			IEngine engine = null;
			if (helper.ViewContext.RouteData.DataTokens.ContainsKey(ContentRoute.ContentEngineKey))
				engine = helper.ViewContext.RouteData.DataTokens[ContentRoute.ContentEngineKey] as IEngine;
			if (engine == null)
				engine = N2.Context.Current;

			return engine.Resolve<T>();
		}

        public static ContentItem CurrentPage(this HtmlHelper helper)
        {
            return helper.CurrentPage<ContentItem>();
        }
        public static T CurrentPage<T>(this HtmlHelper helper) where T : ContentItem
        {
            if (helper == null) throw new ArgumentNullException("helper");

            return helper.ViewContext.CurrentPage<T>();
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

		public static T CurrentPage<T>(this ViewContext context) where T : ContentItem
		{
			if (context == null) throw new ArgumentNullException("context");

			return context.ViewData.CurrentPage<T>()
				?? context.RequestContext.CurrentPage<T>();
		}


		private static T CurrentItem<T>(this ViewDataDictionary viewData) where T : ContentItem
		{
			if (viewData == null) throw new ArgumentNullException("viewData");

			return viewData.Model as T
				?? viewData[ContentRoute.ContentItemKey] as T;
		}
		private static T CurrentPage<T>(this ViewDataDictionary viewData) where T : ContentItem
		{
			if (viewData == null) throw new ArgumentNullException("viewData");

			return viewData[ContentRoute.ContentPageKey] as T;
		}
    }
}
