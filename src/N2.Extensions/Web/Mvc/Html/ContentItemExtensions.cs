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
        public static ContentItem CurrentItem(this HtmlHelper helper)
        {
            return helper.ViewContext.CurrentItem();
        }

        public static ContentItem CurrentItem(this ViewContext context)
        {
            return context.ViewData[ContentRoute.ContentItemKey] as ContentItem
                ?? context.RequestContext.CurrentItem();
        }

        public static ContentItem CurrentItem(this RequestContext context)
        {
            return context.RouteData.Values[ContentRoute.ContentItemKey] as ContentItem;
        }
    }
}
