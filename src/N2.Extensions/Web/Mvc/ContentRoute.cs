using System;
using System.Web.Routing;
using System.Web.Mvc;
using System.Web;
using N2.Engine;
using System.Collections.Generic;

namespace N2.Web.Mvc
{
	/// <summary>
	/// An ASP.NET MVC route that gets route data for content item paths.
	/// </summary>
	public class ContentRoute : RouteBase
	{
		/// <summary>Used to reference the currently executing content item in the route value dictionary.</summary>
		public const string ContentItemKey = "item";
		/// <summary>Used to reference the content page in the route value dictionary.</summary>
		public const string ContentPageKey = "page";
		/// <summary>Used to reference the content part in the route value dictionary.</summary>
		public const string ContentPartKey = "part";
		/// <summary>Used to reference the N2 content engine.</summary>
		public const string ContentEngineKey = "engine";
		/// <summary>Convenience reference to the MVC controller</summary>
		public const string ControllerKey = "controller";
		/// <summary>Convenience reference to the MVC action</summary>
		public const string ActionKey = "action";

		[Obsolete("Unused, will be gone")]
		public const string ContentItemIdKey = "item";
		[Obsolete("Unused, will be gone")]
		public const string ContentPageIdKey = "page";
		[Obsolete("Unused, will be gone")]
		public const string ContentUrlKey = "url";
		
		readonly IEngine engine;
		readonly IRouteHandler routeHandler;
		readonly IControllerMapper controllerMapper;

		readonly List<RouteBase> routes = new List<RouteBase>();

		public ContentRoute(IEngine engine)
			: this(engine, new MvcRouteHandler())
		{
		}

		public ContentRoute(IEngine engine, IRouteHandler routeHandler)
			: this(engine, routeHandler, null)
		{
		}

		public ContentRoute(IEngine engine, IRouteHandler routeHandler, IControllerMapper controllerMapper)
		{
			this.engine = engine;
			this.routeHandler = routeHandler;
			this.controllerMapper = controllerMapper ?? engine.Resolve<IControllerMapper>();

			var defaultRoute = new Route("{controller}/{action}", 
				new RouteValueDictionary(new { action = "Index" }), 
				new RouteValueDictionary(), 
				new RouteValueDictionary(new { engine }), 
				routeHandler);
			routes.Add(defaultRoute);
		}

		public override RouteData GetRouteData(HttpContextBase httpContext)
		{
			string path = httpContext.Request.AppRelativeCurrentExecutionFilePath;
			if (path.StartsWith("~/N2/", StringComparison.InvariantCultureIgnoreCase))
                return new RouteData(this, new StopRoutingHandler());
            if (path.EndsWith(".axd", StringComparison.InvariantCultureIgnoreCase))
                return new RouteData(this, new StopRoutingHandler());
            if (path.EndsWith(".n2.ashx", StringComparison.InvariantCultureIgnoreCase))
                return new RouteData(this, new StopRoutingHandler());

			var routeData = GetRouteDataForPath(httpContext.Request);

			if(routeData != null)
				return routeData;

			var baseRouteData = CheckForContentController(httpContext);

			return baseRouteData;
		}

		private RouteData CheckForContentController(HttpContextBase context)
		{
			var routeData = GetRouteDataFromRoutes(context);

			if(routeData == null)
				return null;

			var controllerName = Convert.ToString(routeData.Values[ControllerKey]);
			var actionName = Convert.ToString(routeData.Values[ActionKey]);

			if (controllerMapper.ControllerHasAction(controllerName, actionName))
			{
				ApplyContent(routeData, context.Request.QueryString, ContentItemKey);
				ApplyContent(routeData, context.Request.QueryString, ContentPageKey);
				routeData.DataTokens[ContentEngineKey] = engine;

				return routeData;
			}

			return null;
		}

		private void ApplyContent(RouteData routeData, System.Collections.Specialized.NameValueCollection query, string key)
		{
			int id;
			if (int.TryParse(query[key], out id))
			{
				routeData.Values[key] = id;
				routeData.DataTokens[key] = engine.Persister.Get(id);
			}
		}

		private int? ParseInteger(string input)
		{
			int output;
			if(int.TryParse(input, out output))
				return output;
			return null;
		}

		private RouteData GetRouteDataFromRoutes(HttpContextBase context)
		{
			foreach (RouteBase route in routes)
			{
				RouteData routeData = route.GetRouteData(context);
				if (routeData != null)
					return routeData;
			}
			return null;
		}

		private RouteData GetRouteDataForPath(HttpRequestBase request)
		{
			//On a multi-lingual site with separate domains per language,
			//the full url (with host) should be passed to UrlParser.ResolvePath():
			string host = (request.Url.IsDefaultPort) ? request.Url.Host : request.Url.Authority;
			string hostAndRawUrl = String.Format("{0}://{1}{2}", request.Url.Scheme, host, request.RawUrl);
			PathData td = engine.UrlParser.ResolvePath(hostAndRawUrl);

			if (td.CurrentItem == null)
				return null;

			var item = td.CurrentItem;
			var page = td.CurrentPage;
			var action = td.Action;
			if (string.IsNullOrEmpty(action))
				action = request.QueryString["action"] ?? "index";

			ContentItem part = null;
			if (!string.IsNullOrEmpty(request.QueryString["part"]))
			{
				// part in query string is used to render a part
				int partId;
				if (int.TryParse(request.QueryString["part"], out partId))
					item = part = engine.Persister.Get(partId);
			}

			if (part == null && item != page && !item.IsPage)
			{
				// isn't a page but wasn't passed via ?part=#, avoid rendering it
				part = item;
				item = page;
			}

			var controllerName = controllerMapper.GetControllerName(item.GetType());

			if (controllerName == null)
				return null;

			if (action == null || !controllerMapper.ControllerHasAction(controllerName, action))
				return null;

			var data = new RouteData(this, routeHandler);

			data.DataTokens[ContentPartKey] = part;
			data.DataTokens[ContentItemKey] = item;
			data.DataTokens[ContentPageKey] = page;
			data.DataTokens[ContentEngineKey] = engine;

			if (part != null)
				data.Values[ContentPartKey] = part.ID;
			data.Values[ContentItemKey] = item.ID;
			data.Values[ContentPageKey] = page.ID;
			
			data.Values[ControllerKey] = controllerName;
			data.Values[ActionKey] = action;

			return data;
		}

		public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
		{
			ContentItem item;

			// try retrieving the item from the route values
			if (!TryConvertContentToController(values, ContentPartKey, out item)
				&& !TryConvertContentToController(values, ContentItemKey, out item)
				&& !TryConvertContentToController(values, ContentPageKey, out item))
			{
				// no item was passed, fallback to current content item
				item = requestContext.CurrentItem();

				if (item == null)
					// no item = no play
					return null;

				string controller = controllerMapper.GetControllerName(item.GetType());
				if (values.ContainsKey(ControllerKey) && !string.Equals(values[ControllerKey] as string, controller, StringComparison.InvariantCultureIgnoreCase))
					// someone's asking for a specific controller so we let another route handle it
					return null;
			}

			if (item.IsPage)
				return ResolveActionUrl(requestContext, values, item);

			// try to find an appropriate page to use as path (part data goes into the query strings)
			ContentItem page;
			if (TryConvertContentToController(values, ContentPageKey, out page))
				// a page parameter was passed
				return ResolvePartActionUrl(requestContext, values, page, item);

			page = requestContext.CurrentPage<ContentItem>();
			if (page != null && page.IsPage)
				// next use the current page
				return ResolvePartActionUrl(requestContext, values, page, item);

			page = item.ClosestPage();
			if (page != null && page.IsPage)
				// fallback to finding the closest page
				return ResolvePartActionUrl(requestContext, values, page, item);

			// can't find a page, don't link
			return null;
		}

		private VirtualPathData ResolvePartActionUrl(RequestContext requestContext, RouteValueDictionary values, ContentItem page, ContentItem item)
		{
			var pageVpd = ResolveActionUrl(requestContext, new RouteValueDictionary { { "action", "index" } }, page);

			if (values.ContainsKey(ControllerKey))
				values.Remove(ControllerKey);
			values[ContentPartKey] = item.ID;

			pageVpd.VirtualPath = Url.Parse(pageVpd.VirtualPath).UpdateQuery(values);
			return pageVpd;
		}

		private VirtualPathData ResolveActionUrl(RequestContext requestContext, RouteValueDictionary values, ContentItem item)
		{
			const string controllerPlaceHolder = "$(CTRL)";
			values[ControllerKey] = controllerPlaceHolder; // pass a placeholder we'll fill with the content path
			VirtualPathData vpd = GetVirtualPathFromRoutes(requestContext, values);
			if (vpd == null)
				return null;

			Url url = item.Url;
			Url actionUrl = vpd.VirtualPath.Replace(controllerPlaceHolder, url.Path);

			vpd.VirtualPath = actionUrl.AppendQuery(url.Query).PathAndQuery.TrimStart('/');
			return vpd;
		}

		private bool TryConvertContentToController(RouteValueDictionary values, string key, out ContentItem item)
		{
			if (!values.ContainsKey(key))
			{
				item = null;
				return false;
			}

			item = values[key] as ContentItem;
			if (item != null)
			{
				// overwrite currently executing controller when "item" is passed
				values.Remove(key);
				values["controller"] = controllerMapper.GetControllerName(item.GetType());
				return true;
			}
			return false;
		}

		private VirtualPathData GetVirtualPathFromRoutes(RequestContext requestContext, RouteValueDictionary values)
		{
			foreach (RouteBase route in routes)
			{
				VirtualPathData vpd = route.GetVirtualPath(requestContext, values);
				if (vpd != null)
					return vpd;
			}
			return null;
		}
	}
}
