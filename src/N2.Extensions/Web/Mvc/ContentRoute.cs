using System;
using System.Web.Routing;
using System.Web.Mvc;
using System.Web;
using N2.Engine;
using N2.Web.Mvc.Html;

namespace N2.Web.Mvc
{
	/// <summary>
	/// An ASP.NET MVC route that gets route data for content item paths.
	/// </summary>
	public class ContentRoute : Route
	{
		public const string ContentItemKey = "item";
		public const string ContentItemIdKey = "id";
		public const string ContentEngineKey = "engine";
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
			: base("{controller}/{action}/{*remainingUrl}", new RouteValueDictionary(new { Action = "Index" }), routeHandler)
		{
			RegisterAdditionalComponents(engine);

			this.engine = engine;
			this.routeHandler = routeHandler;
			this.controllerMapper = controllerMapper ?? engine.Resolve<IControllerMapper>();
		}

		private void RegisterAdditionalComponents(IEngine engine)
		{
			engine.AddComponent("n2.controllerMapper", typeof(IControllerMapper), typeof(ControllerMapper));
			engine.AddComponent("n2.templateRenderer", typeof(ITemplateRenderer), typeof(TemplateRenderer));
		}

		public override RouteData GetRouteData(HttpContextBase httpContext)
		{
			string path = httpContext.Request.AppRelativeCurrentExecutionFilePath;
			if (path.StartsWith("~/edit/", StringComparison.InvariantCultureIgnoreCase))
				return null;
			if (path.EndsWith(".axd", StringComparison.InvariantCultureIgnoreCase))
				return null;

			return GetRouteDataForPath(httpContext.Request.RawUrl);
		}

		public RouteData GetRouteDataForPath(string rawUrl)
		{
			PathData td = engine.UrlParser.ResolvePath(rawUrl);

			if (td.CurrentItem != null)
			{
				RouteData data = new RouteData(this, routeHandler);

				data.Values[ContentItemKey] = td.CurrentItem;
				data.Values[ContentEngineKey] = engine;
				data.Values[ControllerKey] = controllerMapper.GetControllerName(td.CurrentItem.GetType());
				data.Values[ActionKey] = td.Action;
				return data;
			}
			return null;
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

			string requestedController = values[ControllerKey] as string;
			string itemController = controllerMapper.GetControllerName(item.GetType());
			if (!string.Equals(requestedController, itemController, StringComparison.InvariantCultureIgnoreCase))
				return null;

			var pathData = base.GetVirtualPath(requestContext, values);
			Url itemUrl = item.Url;
			Url pathUrl = pathData.VirtualPath;

			if (item.IsPage)
			{
				pathUrl = pathUrl.RemoveSegment(0).PrependSegment(itemUrl.PathWithoutExtension.TrimStart('/'))
					.PathAndQuery.TrimStart('/');
			}
			else
			{
				//pathUrl = pathUrl.PrependSegment(itemUrl.PathWithoutExtension.TrimStart('/'))
				//    .PathAndQuery.TrimStart('/');

				pathUrl = pathUrl.AppendSegment(item.ID.ToString());
			}
			pathData.VirtualPath = pathUrl;

			return pathData;
		}
	}
}
