using System;
using System.Web.Routing;
using System.Web.Mvc;
using System.Web;
using N2.Engine;

namespace N2.Web.Mvc
{
	/// <summary>
	/// An ASP.NET MVC route that gets route data for content item paths.
	/// </summary>
	public class ContentRoute : Route
	{
		public const string ContentItemKey = "n2_item";
		public const string ContentItemIdKey = "n2_itemid";
		public const string ContentEngineKey = "n2_engine";
		public const string ContentUrlKey = "url";
		public const string ControllerKey = "controller";
		public const string ActionKey = "action";

		readonly IEngine engine;
		readonly IRouteHandler routeHandler;
		readonly IControllerMapper controllerMapper;

		public ContentRoute(IEngine engine)
			: this(engine, new MvcRouteHandler())
		{
		}

		public ContentRoute(IEngine engine, IRouteHandler routeHandler)
			: this(engine, routeHandler, null)
		{
		}

		public ContentRoute(IEngine engine, IRouteHandler routeHandler, IControllerMapper controllerMapper)
			: base("{controller}/{action}", new RouteValueDictionary(new { action = "Index" }), routeHandler)
		{
			this.engine = engine;
			this.routeHandler = routeHandler;
			this.controllerMapper = controllerMapper ?? engine.Resolve<IControllerMapper>();
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
			var routeData = base.GetRouteData(context);

			if(routeData == null)
				return null;

			var controllerName = Convert.ToString(routeData.Values[ControllerKey]);
			var actionName = Convert.ToString(routeData.Values[ActionKey]);

			if (controllerMapper.ControllerHasAction(controllerName, actionName))
			{
				routeData.Values[ContentItemIdKey] = context.Request.QueryString[ContentItemIdKey];
				routeData.Values[ContentEngineKey] = engine;

				return routeData;
			}

			return null;
		}

		private RouteData GetRouteDataForPath(HttpRequestBase request)
		{
			PathData td = engine.UrlParser.ResolvePath(request.RawUrl);

			if (td.CurrentItem == null)
				return null;

			var item = td.CurrentItem;
			var action = td.Action;

			if (td.QueryParameters.ContainsKey("preview"))
			{
				int itemId;
				if (Int32.TryParse(td.QueryParameters["preview"], out itemId))
					item = engine.Persister.Get(itemId);
			}
			var controllerName = controllerMapper.GetControllerName(item.GetType());

			if (controllerName == null)
				return null;

			if (!controllerMapper.ControllerHasAction(controllerName, action))
				return null;

			var data = new RouteData(this, routeHandler);
			data.Values[ContentItemKey] = item;
			data.Values[ContentItemIdKey] = item.ID;
			data.Values[ContentEngineKey] = engine;
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
				item = requestContext.RouteData.Values[ContentItemKey] as ContentItem;

			if (item == null)
				return null;

			string itemController = controllerMapper.GetControllerName(item.GetType());
			string requestedController = values[ControllerKey] as string ?? itemController;
			if (!string.Equals(requestedController, itemController, StringComparison.InvariantCultureIgnoreCase))
				return null;

			// pass a placeholder we fill with the content path
			values["controller"] = "$CTRL";
			var pathData = base.GetVirtualPath(requestContext, values);
			Url itemUrl = item.Url;
			Url actionUrl = pathData.VirtualPath.Replace("$CTRL", itemUrl.Path);
			pathData.VirtualPath = actionUrl.AppendQuery(itemUrl.Query).PathAndQuery.TrimStart('/');
			
			return pathData;
		}
	}
}
