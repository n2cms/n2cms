using System.Web.Mvc;
using System.Web.Routing;

namespace N2.Web.Mvc.Html
{
    public static class ContentUrlExtensions
    {
        public static string Action(this UrlHelper url, ContentItem item)
        {
            return Action(url, item, "Index");
        }

        public static string Action(this UrlHelper url, ContentItem item, string action)
        {
            var values = new RouteValueDictionary();
            values[ContentRoute.ContentItemKey] = item.ID != 0 ? (object)item.ID : item;
            return url.Action(action, values);
        }
    }
}
