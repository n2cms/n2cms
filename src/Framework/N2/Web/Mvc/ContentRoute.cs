using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using N2.Engine;

namespace N2.Web.Mvc
{
	/// <summary>
	/// An ASP.NET MVC route that gets route data for content item paths.
	/// </summary>
	public class ContentRoute : RouteBase
	{
		private readonly Engine.Logger<ContentRoute> logger;

		/// <summary>Used to reference the currently executing content item in the route value dictionary.</summary>
		public static string ContentItemKey
		{
			get { return PathData.ItemQueryKey; }
		}
		/// <summary>Used to reference the content page in the route value dictionary.</summary>
		public static string ContentPageKey
		{
			get { return PathData.PageQueryKey; }
		}
		/// <summary>Used to reference the content part in the route value dictionary.</summary>
		public static string ContentPartKey
		{
			get { return PathData.PartQueryKey; }
		}

		/// <summary>Used to reference the N2 content engine.</summary>
		public const string ContentEngineKey = "engine";
		/// <summary>Convenience reference to the MVC controller.</summary>
		public const string ControllerKey = "controller";
		/// <summary>Convenience reference to the MVC area.</summary>
		public const string AreaKey = "area";
		/// <summary>Convenience reference to the MVC action.</summary>
		public const string ActionKey = "action";
		
		readonly IEngine engine;
		readonly IRouteHandler routeHandler;
		readonly IControllerMapper controllerMapper;
		readonly Route innerRoute;
		string managementPath;

		public ContentRoute(IEngine engine)
			: this(engine, null, null, null)
		{
		}

		public ContentRoute(IEngine engine, IRouteHandler routeHandler, IControllerMapper controllerMapper, Route innerRoute, string[] namespaces = null)
		{
			managementPath = Url.ToRelative(Url.ResolveTokens(Url.ManagementUrlToken + "/"));
			this.engine = engine;
			this.routeHandler = routeHandler ?? new MvcRouteHandler();
			this.controllerMapper = controllerMapper ?? engine.Resolve<IControllerMapper>();
			this.innerRoute = innerRoute ?? new Route("{controller}/{action}", 
				new RouteValueDictionary(new { action = "Index" }), 
				new RouteValueDictionary(),
				new RouteValueDictionary(new { this.engine, namespaces }), 
				this.routeHandler);
		}

		/// <summary>Gets route data for for items this route handles.</summary>
		/// <param name="item">The item whose route to get.</param>
		/// <param name="routeValues">The route values to apply to the route data.</param>
		/// <returns>A route data object or null.</returns>
		public virtual RouteValueDictionary GetRouteValues(ContentItem item, RouteValueDictionary routeValues)
		{
			string actionName = "Index";
			if (routeValues.ContainsKey(ActionKey))
				actionName = (string)routeValues[ActionKey];

			string controllerName = controllerMapper.GetControllerName(item.GetContentType());
			if (controllerName == null || !controllerMapper.ControllerHasAction(controllerName, actionName))
				return null;

			var values = new RouteValueDictionary(routeValues);

			foreach (var kvp in innerRoute.Defaults)
				if(!values.ContainsKey(kvp.Key))
					values[kvp.Key] = kvp.Value;

			values[ControllerKey] = controllerName;
			values[ActionKey] = actionName;
			values[ContentItemKey] = item.ID;
			values[AreaKey] = innerRoute.DataTokens["area"];

			return values;
		}

		public override RouteData GetRouteData(HttpContextBase httpContext)
		{
			string path = httpContext.Request.AppRelativeCurrentExecutionFilePath;
			if (path.StartsWith(managementPath, StringComparison.InvariantCultureIgnoreCase))
				return new RouteData(this, new StopRoutingHandler());
			if (path.EndsWith(".axd", StringComparison.InvariantCultureIgnoreCase))
				return new RouteData(this, new StopRoutingHandler());
			if (path.EndsWith(".ashx", StringComparison.InvariantCultureIgnoreCase))
				return new RouteData(this, new StopRoutingHandler());

			RouteData routeData = null;

			if(httpContext.Request.QueryString[ContentPartKey] != null)
				// part in query string, this is an indicator of a request to a part, takes precendence over friendly urls
				routeData = CheckForContentController(httpContext);

			if(routeData == null)
				// this might be a friendly url
				routeData = GetRouteDataForPath(httpContext.Request);

			if(routeData == null)
				// fallback to route to controller/action
				routeData = CheckForContentController(httpContext);

			logger.DebugFormat("GetRouteData for '{0}' got values: {1}", path, new RouteExtensions.QueryStringOutput(routeData));

			return routeData;
		}

		private RouteData GetRouteDataForPath(HttpRequestBase request)
		{
			//On a multi-lingual site with separate domains per language,
			//the full url (with host) should be passed to UrlParser.ResolvePath():
			string host = (request.Url.IsDefaultPort) ? request.Url.Host : request.Url.Authority;
			var url = new Url(request.Url.Scheme, host, request.RawUrl);
			PathData path = engine.Resolve<RequestPathProvider>().ResolveUrl(url);

			if (!path.IsEmpty() && path.IsRewritable && StopRewritableItems)
				return new RouteData(this, new StopRoutingHandler());

			var page = path.CurrentPage;

			var actionName = path.Action;
			if (string.IsNullOrEmpty(actionName))
				actionName = request.QueryString["action"] ?? "Index";

			if (!string.IsNullOrEmpty(request.QueryString[PathData.PageQueryKey]))
			{
				int pageId;
				if (int.TryParse(request.QueryString[PathData.PageQueryKey], out pageId))
				{
					path.CurrentPage = page = engine.Persister.Get(pageId);
				}
			}

			ContentItem part = null;
			if (!string.IsNullOrEmpty(request.QueryString[PathData.PartQueryKey]))
			{
				// part in query string is used to render a part
				int partId;
				if (int.TryParse(request.QueryString[PathData.PartQueryKey], out partId))
				{
					path.CurrentItem = part = engine.Persister.Get(partId);
					path.Controller = null;
				}
			}

			if (!string.IsNullOrEmpty(request.QueryString[PathData.ItemQueryKey]))
			{
				// this is a discrepancy between mvc and the legacy
				// in mvc the item query key doesn't route to the item, it's a marker
				// the urlparser however parses the item query key and passes the item as current in pathdata
				// hence this somewhat strange sidestepping
				int itemId;
				if (int.TryParse(request.QueryString[PathData.ItemQueryKey], out itemId))
				{
					if (itemId == path.ID)
					{
						// we have an item id and it matches the path data we found via url parser
						if (part == null || part.ID != itemId)
						{
							// it hasn't been changed by a specific part query string so we reset it
							path.CurrentItem = path.CurrentPage;
							path.Controller = null;
						}
					}
				}
			}

			if (page == null && part == null)
				return null;
			
			path.CurrentPage = page ?? part.ClosestPage();

			var controllerName = path.Controller 
				?? controllerMapper.GetControllerName((part ?? page).GetContentType());

			if (controllerName == null)
				return null;

			if (actionName == null || !controllerMapper.ControllerHasAction(controllerName, actionName))
				return null;

			var data = new RouteData(this, routeHandler);

			foreach (var defaultPair in innerRoute.Defaults)
				data.Values[defaultPair.Key] = defaultPair.Value;
			foreach (var tokenPair in innerRoute.DataTokens)
				data.DataTokens[tokenPair.Key] = tokenPair.Value;

			RouteExtensions.ApplyCurrentPath(data, controllerName, actionName, path);
			data.DataTokens[ContentEngineKey] = engine;

			return data;
		}

		/// <summary>Responds to the path /{controller}/{action}/?page=123&item=234</summary>
		private RouteData CheckForContentController(HttpContextBase context)
		{
			var routeData = innerRoute.GetRouteData(context);

			if (routeData == null)
				return null;
			routeData.Route = this;
			routeData.RouteHandler = routeHandler;

			var controllerName = Convert.ToString(routeData.Values[ControllerKey]);
			var actionName = Convert.ToString(routeData.Values[ActionKey]);

			if (!controllerMapper.ControllerHasAction(controllerName, actionName))
				return null;

			var part = ResolveContent(context.Request.QueryString, ContentPartKey)
				?? ResolveContent(context.Request.QueryString, ContentItemKey);
			var page = ResolveContent(context.Request.QueryString, ContentPageKey);

			if (part == null && page == null)
				return null;

			routeData.ApplyCurrentPath(new PathData(page ?? Find.ClosestPage(part), part));
			routeData.DataTokens[ContentEngineKey] = engine;

			return routeData;
		}

		private ContentItem ResolveContent(NameValueCollection query, string key)
		{
			int id;
			if (int.TryParse(query[key], out id))
			{
				var item = engine.Persister.Get(id);
				return item;
			}
			return null;
		}



		public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
		{
			// here we deal with people linking to non-n2 controllers but we have merged route values containing n2 stuff
			// scenarios:
			// controller: other -> not our business
			// controller: self -> link to same acton and page
			// acton: other -> link to action on same page
			// item: other -> link to other page
			// nuthin' -> link to same acton and page
			// x: y -> add parameter x
			// page: other -> not our business
			// item: other & action: other -> link to action other page
			// item: other & action: other & x:y-> link to action other page with param
			// app in virtual dir

			values = new RouteValueDictionary(values);

			logger.DebugFormat("GetVirtualPath for values: {0}", new RouteExtensions.QueryStringOutput(values));

			var contextPath = requestContext.RouteData.CurrentPath();
			var requestedItem = values.CurrentItem<ContentItem>(ContentItemKey, engine.Persister);
			var item = requestedItem;

			if (item == null)
				// fallback to context item
				item = contextPath.CurrentItem;
			else
				// remove specified item from collection so it doesn't appear in the url
				values.Remove(ContentItemKey);

			if (item == null)
				// no item requested or in context .> not our bisiness
				return null;

			var contextController = (string)requestContext.RouteData.Values["controller"];
			var requestedController = (string)values["controller"];
            if (requestedItem == null && requestedController != null)
            {
                if (!string.Equals(requestedController, contextController, StringComparison.InvariantCultureIgnoreCase))
                    // no item was specificlly requested, and the controller differs from context's -> we let some other route handle this
                    return null;

                if (!controllerMapper.IsContentController(requestedController))
                    // same controller not content controller -> let it be
                    return null;
            }

			var itemController = controllerMapper.GetControllerName(item.GetContentType());
			values["controller"] = itemController;

			if (item.IsPage)
				return ResolveContentActionUrl(requestContext, values, item);

			// try to find an appropriate page to use as path (part data goes into the query strings)
			var page = values.CurrentItem<ContentItem>(ContentPageKey, engine.Persister)
				?? contextPath.CurrentPage
				?? item.ClosestPage();

			if (page != null)
				return ResolvePartActionUrl(requestContext, values, page, item);

			// can't find a page, don't link
			return null;
		}

		private VirtualPathData ResolvePartActionUrl(RequestContext requestContext, RouteValueDictionary values, ContentItem page, ContentItem item)
		{
			values[ContentPageKey] = page.ID;
			values[ContentPartKey] = item.ID;
			var vpd = innerRoute.GetVirtualPath(requestContext, values);
			if (vpd == null)
				return null;
			
			vpd.Route = this;
			vpd.DataTokens[PathData.PathKey] = new PathData(page, item);
			return vpd;
		}

		private VirtualPathData ResolveContentActionUrl(RequestContext requestContext, RouteValueDictionary values, ContentItem item)
		{
			const string controllerPlaceHolder = "---(CTRL)---";
			const string areaPlaceHolder = "---(AREA)---";
		
			values[ControllerKey] = controllerPlaceHolder; // pass a placeholder we'll fill with the content path
			bool useAreas = innerRoute.DataTokens.ContainsKey("area");
			if (useAreas)
				values[AreaKey] = areaPlaceHolder;
			if (values.ContainsKey(ContentItemKey))
				values.Remove(ContentItemKey);

			VirtualPathData vpd = innerRoute.GetVirtualPath(requestContext, values);
			if (vpd == null)
				return null;
			vpd.Route = this;
			vpd.DataTokens[PathData.PathKey] = new PathData(item);

			string relativeUrl = Url.ToRelative(item.Url);
			Url actionUrl = vpd.VirtualPath
				.Replace(controllerPlaceHolder, Url.PathPart(relativeUrl).TrimStart('~'));
			if (useAreas)
				actionUrl = actionUrl.SetPath(actionUrl.Path.Replace(areaPlaceHolder + "/", ""));

			foreach (var kvp in Url.ParseQueryString(Url.QueryPart(relativeUrl)))
			{
				if ("item".Equals(kvp.Key, StringComparison.InvariantCultureIgnoreCase))
					continue;

				actionUrl = actionUrl.AppendQuery(kvp.Key, kvp.Value);
			}
			vpd.VirtualPath = actionUrl.PathAndQuery.TrimStart('/');
			return vpd;
		}

		/// <summary>Make the route table stop at items that match an item that can be rewritten to (probably webforms).</summary>
		public bool StopRewritableItems { get; set; }
	}
}
