using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace N2.Web.Mvc.Html
{
	public static class LinkExtensions
	{
		public static MvcHtmlString ActionLink(this HtmlHelper htmlHelper, ContentItem item)
		{
			return htmlHelper.ActionLink(item, "Index");
		}
		public static MvcHtmlString ActionLink(this HtmlHelper htmlHelper, ContentItem item, string action)
		{
			return htmlHelper.ActionLink(item.Title, action, null, new RouteValueDictionary() { { "item", item } }, new RouteValueDictionary());
		}
		public static MvcHtmlString ActionLink(this HtmlHelper htmlHelper, ContentItem item, object routeValues)
		{
			return htmlHelper.ActionLink(item, "Index", routeValues);
		}
		public static MvcHtmlString ActionLink(this HtmlHelper htmlHelper, ContentItem item, string action, object routeValues)
		{
			return htmlHelper.ActionLink(item.Title, action, null, new RouteValueDictionary(routeValues) { { "item", item } }, new RouteValueDictionary());
		}
		public static MvcHtmlString ActionLink(this HtmlHelper htmlHelper, ContentItem item, object routeValues, object htmlAttributes)
		{
			return htmlHelper.ActionLink(item, "Index", routeValues, htmlAttributes);
		}
		public static MvcHtmlString ActionLink(this HtmlHelper htmlHelper, ContentItem item, string action, object routeValues, object htmlAttributes)
		{
			return htmlHelper.ActionLink(item.Title, action, null, new RouteValueDictionary(routeValues) { { "item", item } }, new RouteValueDictionary(htmlAttributes));
		}
		public static MvcHtmlString ActionLink(this HtmlHelper htmlHelper, string linkText, ContentItem item, object routeValues, object htmlAttributes)
		{
			return htmlHelper.ActionLink(linkText, "Index", null, new RouteValueDictionary(routeValues) { { "item", item } }, new RouteValueDictionary(htmlAttributes));
		}
		public static MvcHtmlString ActionLink(this HtmlHelper htmlHelper, string linkText, ContentItem item, string action, object routeValues, object htmlAttributes)
		{
			return htmlHelper.ActionLink(linkText, action, null, new RouteValueDictionary(routeValues) { { "item", item } }, new RouteValueDictionary(htmlAttributes));
		}
	}
}
