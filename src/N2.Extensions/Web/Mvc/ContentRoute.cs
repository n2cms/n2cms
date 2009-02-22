using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
using System.Web.Mvc;
using System.Web;
using N2.Engine;
using N2.Definitions;

namespace N2.Web.Mvc
{
	/// <summary>
	/// An ASP.NET MVC route that gets route data for content item paths.
	/// </summary>
	public class ContentRoute : Route
	{
		public const string ContentItemKey = "item";
		public const string ContentEngineKey = "engine";
		public const string ContentUrlKey = "url";
		public const string ControllerKey = "controller";
		public const string ActionKey = "action";

		readonly IEngine engine;
		readonly IRouteHandler routeHandler;
		readonly IDictionary<Type, string> controllerMap = new Dictionary<Type, string>();

		public ContentRoute(IEngine engine)
			: this(engine, new MvcRouteHandler())
		{
		}

		public ContentRoute(IEngine engine, IRouteHandler routeHandler)
			: base("{controller}/{action}/{*remainingUrl}", new RouteValueDictionary(new { Action = "Index" }), routeHandler)
		{
			this.engine = engine;
			this.routeHandler = routeHandler;

			IList<ControlsAttribute> controllerDefinitions = FindControllers(engine);
			foreach (ItemDefinition id in engine.Definitions.GetDefinitions())
			{
				IControllerDescriptor controllerDefinition = GetControllerFor(id.ItemType, controllerDefinitions);
				ControllerMap[id.ItemType] = controllerDefinition.ControllerName;
				IList<IPathFinder> finders = ContentItem.GetPathFinders(id.ItemType);
				if (0 == finders.Where(f => f is RouteActionResolverAttribute || f is ActionResolver).Count())
				{
					var methods = controllerDefinition.ControllerType.GetMethods().Select(m => m.Name).ToArray();
					var actionResolver = new ActionResolver(methods);
					finders.Insert(0, actionResolver);
					FinderDictionary[id.ItemType] = finders;
				}
			}
		}

		public override RouteData GetRouteData(HttpContextBase httpContext)
		{
			string path = httpContext.Request.AppRelativeCurrentExecutionFilePath;
			if (path.StartsWith("~/edit/", StringComparison.InvariantCultureIgnoreCase))
				return null;
			if (path.EndsWith(".axd", StringComparison.InvariantCultureIgnoreCase))
				return null;

			PathData td = engine.UrlParser.ResolvePath(httpContext.Request.RawUrl);

			if (td.CurrentItem != null)
			{
				RouteData data = new RouteData(this, routeHandler);

				data.Values[ContentItemKey] = td.CurrentItem;
				data.Values[ContentEngineKey] = engine;
				data.Values[ControllerKey] = GetControllerName(td.CurrentItem.GetType());
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
			string itemController = GetControllerName(item.GetType());
			if (!string.Equals(requestedController, itemController, StringComparison.InvariantCultureIgnoreCase))
				return null;

			var pathData = base.GetVirtualPath(requestContext, values);
			Url itemUrl = item.Url;
			Url pathUrl = pathData.VirtualPath;
			pathData.VirtualPath = pathUrl.RemoveSegment(0).PrependSegment(itemUrl.PathWithoutExtension.TrimStart('/')).PathAndQuery.TrimStart('/');
			
			return pathData;
		}

		IDictionary<Type, IList<IPathFinder>> FinderDictionary
		{
			get { return SingletonDictionary<Type, IList<IPathFinder>>.Instance; }
		}
		public IDictionary<Type, string> ControllerMap
		{
			get { return controllerMap; }
		}

		private string GetControllerName(Type type)
		{
			return ControllerMap[type];
		}

		private IControllerDescriptor GetControllerFor(Type itemType, IList<ControlsAttribute> controllerDefinitions)
		{
			foreach (ControlsAttribute controllerDefinition in controllerDefinitions)
			{
				if (controllerDefinition.ItemType.IsAssignableFrom(itemType))
				{
					return controllerDefinition;
				}
			}
			throw new N2Exception("Found no controller for type '" + itemType + "' among " + controllerDefinitions.Count + " found controllers.");
		}

		private IList<ControlsAttribute> FindControllers(IEngine engine)
		{
			List<ControlsAttribute> controllerDefinitions = new List<ControlsAttribute>();
			foreach (Type controllerType in engine.Resolve<ITypeFinder>().Find(typeof(IController)))
			{
				foreach (ControlsAttribute attr in controllerType.GetCustomAttributes(typeof(ControlsAttribute), false))
				{
					attr.ControllerType = controllerType;
					controllerDefinitions.Add(attr);
				}
			}
			controllerDefinitions.Sort();
			return controllerDefinitions;
		}
	}
}
