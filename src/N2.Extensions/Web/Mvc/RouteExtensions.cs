using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;
using N2.Persistence;
using System.Web;
using System.Web.Mvc;
using System.Linq.Expressions;

namespace N2.Web.Mvc
{
	public static class RouteExtensions
	{
		public static RouteData ApplyCurrentItem(this RouteData data, string controllerName, string actionName, ContentItem page, ContentItem part)
		{
			data.Values[ContentRoute.ControllerKey] = controllerName;
			data.Values[ContentRoute.ActionKey] = actionName;

			return data.ApplyContentItem(ContentRoute.ContentPageKey, page)
				.ApplyContentItem(ContentRoute.ContentPartKey, part);
		}

		internal static RouteData ApplyContentItem(this RouteData data, string key, ContentItem item)
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

		public static ContentItem CurrentItem(this RouteData routeData)
		{
			return routeData.DataTokens.CurrentItem(ContentRoute.ContentPartKey)
				?? routeData.DataTokens.CurrentItem(ContentRoute.ContentPageKey);
		}

		public static ContentItem CurrentPage(this RouteData routeData)
		{
			return routeData.DataTokens.CurrentItem(ContentRoute.ContentPageKey);
		}

		internal static ContentItem CurrentItem(this RouteValueDictionary data, string key)
		{
			if (data.ContainsKey(key))
				return data[key] as ContentItem;
			return null;
		}

		public static T CurrentItem<T>(this RouteValueDictionary data, string key, IPersister persister)
			where T: ContentItem
		{
			if (data.ContainsKey(key))
			{
				object value = data[key];
				if (value == null)
					return null;
				
				var item = value as T;
				if (item != null)
					return item as T;

				if(value is int)
					return persister.Get((int)value) as T;

				int itemId;
				if (value is string && int.TryParse(value as string, out itemId))
					return persister.Get((int)value) as T;
			}

			return null;
		}
	}
}
