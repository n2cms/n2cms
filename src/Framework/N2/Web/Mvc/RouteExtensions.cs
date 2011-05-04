using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;
using N2.Persistence;
using System.Web;
using System.Web.Mvc;
using System.Linq.Expressions;
using N2.Engine;

namespace N2.Web.Mvc
{
	public static class RouteExtensions
	{
		public static IEngine GetEngine(RouteData routeData)
		{
			return routeData.DataTokens[ContentRoute.ContentEngineKey] as IEngine
				?? N2.Context.Current;
		}

		public static T ResolveService<T>(RouteData routeData) where T : class
		{
			return GetEngine(routeData).Resolve<T>();
		}

		public static T[] ResolveServices<T>(RouteData routeData) where T : class
		{
			return GetEngine(routeData).Container.ResolveAll<T>();
		}



		/// <summary>Applies existing or creates external content on the route data.</summary>
		/// <param name="data">The data that will receive external content.</param>
		/// <param name="family">The group to associate external content to.</param>
		/// <param name="key">The key of this particular external content.</param>
		/// <param name="url">The url on which this external content item is displayed.</param>
		/// <returns>The external content item itself.</returns>
		public static ContentItem ApplyExternalContent(this RouteData data, string family, string key, string url)
		{
			var item = ResolveService<Edit.IExternalContentRepository>(data).GetOrCreate(family, key, url);
			data.ApplyContentItem(ContentRoute.ContentPageKey, item);
			return item;
		}

		/// <summary>Applies the current content item and controller to the route data.</summary>
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
