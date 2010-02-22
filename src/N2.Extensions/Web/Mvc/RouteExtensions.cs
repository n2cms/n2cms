using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;

namespace N2.Web.Mvc
{
	public static class RouteExtensions
	{
		public static RouteData ApplyCurrentItem(this RouteData data, string controllerName, string actionName, ContentItem item, ContentItem page, ContentItem part)
		{
			data.Values[ContentRoute.ControllerKey] = controllerName;
			data.Values[ContentRoute.ActionKey] = actionName;

			return data.ApplyContentItem(ContentRoute.ContentItemKey, item)
				.ApplyContentItem(ContentRoute.ContentPageKey, page)
				.ApplyContentItem(ContentRoute.ContentPartKey, part);
		}

		public static RouteData ApplyContentItem(this RouteData data, string key, ContentItem item)
		{
			if (data == null) throw new ArgumentNullException("data");
			if (key == null) throw new ArgumentNullException("key");

			if (item != null)
			{
				data.DataTokens[key] = item;
				data.Values[key] = item.ID;
			}

			return data;
		}
	}
}
