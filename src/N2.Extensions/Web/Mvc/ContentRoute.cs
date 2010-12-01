using System;
using System.Web.Routing;
using System.Web.Mvc;
using System.Web;
using N2.Engine;
using System.Collections.Specialized;
using System.Diagnostics;

namespace N2.Web.Mvc
{
	/// <summary>
	/// An ASP.NET MVC route that gets route data for content item paths.
	/// </summary>
	public class ContentRoute : RouteBase
	{
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

		public ContentRoute(IEngine engine)
			: this(engine, null, null, null)
		{
		}

		public ContentRoute(IEngine engine, IRouteHandler routeHandler, IControllerMapper controllerMapper, Route innerRoute)
		{
			this.engine = engine;
			this.routeHandler = routeHandler ?? new MvcRouteHandler();
			this.controllerMapper = controllerMapper ?? engine.Resolve<IControllerMapper>();
			this.innerRoute = innerRoute ?? new Route("{controller}/{action}", 
				new RouteValueDictionary(new { action = "Index" }), 
				new RouteValueDictionary(), 
				new RouteValueDictionary(new { this.engine }), 
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
			if (path.StartsWith(engine.ManagementPaths.GetManagementInterfaceUrl(), StringComparison.InvariantCultureIgnoreCase))
				return new RouteData(this, new StopRoutingHandler());
			if (path.EndsWith(".axd", StringComparison.InvariantCultureIgnoreCase))
				return new RouteData(this, new StopRoutingHandler());
			if (path.EndsWith(".n2.ashx", StringComparison.InvariantCultureIgnoreCase))
				return new RouteData(this, new StopRoutingHandler());

			RouteData routeData = null;

			// part in query string, this is an indicator of a request to a part
			if(httpContext.Request.QueryString[ContentPartKey] != null)
				routeData = CheckForContentController(httpContext);

			// this might be a friendly url
			if(routeData == null)
				routeData = GetRouteDataForPath(httpContext.Request);

			// fallback to route to controller/action
			if(routeData == null)
				routeData = CheckForContentController(httpContext);

			Debug.WriteLine("GetRouteData for '" + path + "' got values: " + (routeData != null ? routeData.Values.ToQueryString() : "(null)"));
			return routeData;
		}

		private RouteData GetRouteDataForPath(HttpRequestBase request)
		{
			//On a multi-lingual site with separate domains per language,
			//the full url (with host) should be passed to UrlParser.ResolvePath():
			string host = (request.Url.IsDefaultPort) ? request.Url.Host : request.Url.Authority;
			string hostAndRawUrl = String.Format("{0}://{1}{2}", request.Url.Scheme, host, Url.ToAbsolute(request.AppRelativeCurrentExecutionFilePath));
			PathData td = engine.UrlParser.ResolvePath(hostAndRawUrl);

			var page = td.CurrentPage;

			var actionName = td.Action;
			if (string.IsNullOrEmpty(actionName))
				actionName = request.QueryString["action"] ?? "Index";

			if (!string.IsNullOrEmpty(request.QueryString[PathData.PageQueryKey]))
			{
				int pageId;
				if (int.TryParse(request.QueryString[PathData.PageQueryKey], out pageId))
					td.CurrentPage = page = engine.Persister.Get(pageId);
			}

			ContentItem part = null;
			if (!string.IsNullOrEmpty(request.QueryString[PathData.PartQueryKey]))
			{
				// part in query string is used to render a part
				int partId;
				if (int.TryParse(request.QueryString[PathData.PartQueryKey], out partId))
					td.CurrentItem = part = engine.Persister.Get(partId);
			}

			if (page == null && part == null)
				return null;
			else if (page == null)
				page = part.ClosestPage();

			var controllerName = controllerMapper.GetControllerName((part ?? page).GetContentType());

			if (controllerName == null)
				return null;

			if (actionName == null || !controllerMapper.ControllerHasAction(controllerName, actionName))
				return null;

			var data = new RouteData(this, routeHandler);

			foreach (var defaultPair in innerRoute.Defaults)
				data.Values[defaultPair.Key] = defaultPair.Value;
			foreach (var tokenPair in innerRoute.DataTokens)
				data.DataTokens[tokenPair.Key] = tokenPair.Value;

			data.ApplyCurrentItem(controllerName, actionName, page, part);
			data.DataTokens[ContentEngineKey] = engine;

			return data;
		}

		/// <summary>Responds to the path /{controller}/{action}/?page=123&item=234</summary>
		private RouteData CheckForContentController(HttpContextBase context)
		{
			var routeData = innerRoute.GetRouteData(context);

			if (routeData == null)
				return null;

			var controllerName = Convert.ToString(routeData.Values[ControllerKey]);
			var actionName = Convert.ToString(routeData.Values[ActionKey]);

			if (!controllerMapper.ControllerHasAction(controllerName, actionName))
				return null;

			
			var part = ApplyContent(routeData, context.Request.QueryString, ContentPartKey);
			if (part != null)
				routeData.ApplyContentItem(ContentItemKey, part);
			else
				ApplyContent(routeData, context.Request.QueryString, ContentItemKey);
			
			var page = ApplyContent(routeData, context.Request.QueryString, ContentPageKey);
			if (page == null)
				routeData.ApplyContentItem(ContentPageKey, part.ClosestPage());
			routeData.DataTokens[ContentEngineKey] = engine;

			return routeData;
		}

		private ContentItem ApplyContent(RouteData routeData, NameValueCollection query, string key)
		{
			int id;
			if (int.TryParse(query[key], out id))
			{
				var item = engine.Persister.Get(id);
				routeData.ApplyContentItem(key, item);
				return item;
			}
			return null;
		}



		public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
		{
			ContentItem item;

			Debug.WriteLine("GetVirtualPath for values: " + values.ToQueryString());

			values = new RouteValueDictionary(values);

			// try retrieving the item from the route values
			if (!TryConvertContentToController(requestContext, values, ContentItemKey, out item))
			{
				// no item was passed, fallback to current content item
				item = requestContext.CurrentItem();

				if (item == null)
					// no item = no play
					return null;

				if (!RequestedControllerMatchesItemController(values, item))
					// someone's asking for a specific controller so we let another route handle it
					return null;
			}

			if (item.IsPage)
				return ResolveContentActionUrl(requestContext, values, item);

			// try to find an appropriate page to use as path (part data goes into the query strings)
			ContentItem page = values.CurrentItem<ContentItem>(ContentPageKey, engine.Persister);
			if (page != null)
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

		private bool RequestedControllerMatchesItemController(RouteValueDictionary values, ContentItem item)
		{
			string requestedController = values[ControllerKey] as string;
			if (requestedController == null)
				return true;

			string itemController = controllerMapper.GetControllerName(item.GetContentType());

			return string.Equals(requestedController, itemController, StringComparison.InvariantCultureIgnoreCase);
		}

		private VirtualPathData ResolvePartActionUrl(RequestContext requestContext, RouteValueDictionary values, ContentItem page, ContentItem item)
		{
			values[ContentPageKey] = page.ID;
			values[ContentPartKey] = item.ID;
			var vpd = innerRoute.GetVirtualPath(requestContext, values);
			if (vpd != null)
				vpd.Route = this;
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

			VirtualPathData vpd = innerRoute.GetVirtualPath(requestContext, values);
			if (vpd == null)
				return null;
			vpd.Route = this;

			string relativeUrl = Url.ToRelative(item.Url);
			Url actionUrl = vpd.VirtualPath
				.Replace(controllerPlaceHolder, Url.PathPart(relativeUrl).TrimStart('~'));
			if (useAreas)
				actionUrl = actionUrl.SetPath(actionUrl.Path.Replace(areaPlaceHolder + "/", ""));

			foreach (var kvp in Url.ParseQueryString(Url.QueryPart(relativeUrl)))
			{
				if ("item".Equals(kvp.Value, StringComparison.InvariantCultureIgnoreCase))
					continue;

				actionUrl = actionUrl.AppendQuery(kvp.Key, kvp.Value);
			}
			vpd.VirtualPath = actionUrl.PathAndQuery.TrimStart('/');
			return vpd;
		}

		private bool TryConvertContentToController(RequestContext request, RouteValueDictionary values, string key, out ContentItem item)
		{
			if (!values.ContainsKey(key))
			{
				item = null;
				return false;
			}

			object value = values[key];
			item = value as ContentItem;
			if (item == null && value is int)
			{
				item = engine.Persister.Get((int)value);
			}

			if (item == null || item == request.CurrentItem())
				// got item from merged values
				return false;

			// replace requested controller when other "item" is passed
			values.Remove(key);
			values[ControllerKey] = controllerMapper.GetControllerName(item.GetContentType());
		
			return true;
		}
	}
}
