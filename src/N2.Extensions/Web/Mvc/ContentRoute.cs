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
		public const string ContentItemKey = "n2_item";
		public const string ContentPageKey = "n2_page";
		public const string ContentItemIdKey = "n2_itemid";
		public const string ContentPageIdKey = "n2_pageid";
		public const string ContentEngineKey = "n2_engine";
		public const string ContentUrlKey = "url";
		public const string ControllerKey = "controller";
		public const string ActionKey = "action";

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
			if (path.StartsWith("~/N2/Content/", StringComparison.InvariantCultureIgnoreCase))
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
				ApplyContent(routeData, context.Request.QueryString, ContentItemIdKey, ContentItemKey);
				ApplyContent(routeData, context.Request.QueryString, ContentPageIdKey, ContentPageKey);
				routeData.DataTokens[ContentEngineKey] = engine;

				return routeData;
			}

			return null;
		}

		private void ApplyContent(RouteData routeData, System.Collections.Specialized.NameValueCollection query, string idKey, string contentKey)
		{
			string idValue = query[idKey];
			if (!string.IsNullOrEmpty(idValue))
			{
				int id = int.Parse(idValue);
				routeData.Values[idKey] = id;
				routeData.DataTokens[contentKey] = engine.Persister.Get(id);
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
			PathData td = engine.UrlParser.ResolvePath(request.RawUrl);

			if (td.CurrentItem == null)
				return null;

			var part = td.CurrentItem;
			var page = td.CurrentPage;
			var action = td.Action;

			if (td.QueryParameters.ContainsKey("preview"))
			{
				int itemId;
				if (Int32.TryParse(td.QueryParameters["preview"], out itemId))
					part = engine.Persister.Get(itemId);
			}

			var controllerName = controllerMapper.GetControllerName(part.GetType());

			if (controllerName == null)
				return null;

			if (action == null || !controllerMapper.ControllerHasAction(controllerName, action))
				return null;

			var data = new RouteData(this, routeHandler);
			data.DataTokens[ContentItemKey] = part;
			data.Values[ContentItemIdKey] = part.ID;
			data.DataTokens[ContentPageKey] = page;
			data.Values[ContentPageIdKey] = page.ID;
			data.DataTokens[ContentEngineKey] = engine;
			data.Values[ControllerKey] = controllerName;
			data.Values[ActionKey] = action;

			return data;
		}

		public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
		{
			ContentItem item;
			if(values.ContainsKey(ContentItemKey))
			{
				item = values[ContentItemKey] as ContentItem;
				values.Remove(ContentItemKey);
			}
			else
				item = requestContext.RouteData.DataTokens[ContentItemKey] as ContentItem;

			if (item == null)
				return null;

			string controller = controllerMapper.GetControllerName(item.GetType());
			if (values.ContainsKey(ControllerKey) && !string.Equals(values[ControllerKey] as string, controller, StringComparison.InvariantCultureIgnoreCase))
				// someone's asking for a specific controller so we let another route handle it
				return null;

			if (item.IsPage)
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
			else
			{
				values[ControllerKey] = controller;
				values[ContentItemIdKey] = item.ID;
				return GetVirtualPathFromRoutes(requestContext, values);
			}
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
