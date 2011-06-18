﻿using System;
using System.Linq;
using System.Web.Mvc;
using N2.Definitions;

namespace N2.Web.Mvc.Html
{
    public static class ContentItemExtensions
	{
		public static PathData CurrentPath(this HtmlHelper helper)
		{
			return new PathData { CurrentItem = helper.CurrentItem<ContentItem>(), CurrentPage = helper.CurrentPage<ContentItem>() };
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

			return viewData[ContentRoute.ContentItemKey] as T
				?? viewData.Model as T;
		}
		private static T CurrentPage<T>(this ViewDataDictionary viewData) where T : ContentItem
		{
			if (viewData == null) throw new ArgumentNullException("viewData");

			return viewData[ContentRoute.ContentPageKey] as T;
		}



		public static ContentItem StartPage(this HtmlHelper html)
		{
			return Find.EnumerateParents(html.CurrentItem(), null, true).FirstOrDefault(i => i is IStartPage)
				?? N2.Find.StartPage;
		}

		public static T StartPage<T>(this HtmlHelper html) where T : ContentItem
		{
			return Find.Closest<T>(html.CurrentItem())
				?? N2.Find.StartPage as T;
		}

		public static ContentItem RootPage(this HtmlHelper html)
		{
			return Find.EnumerateParents(html.CurrentItem(), null, true).LastOrDefault();
		}
    }
}
