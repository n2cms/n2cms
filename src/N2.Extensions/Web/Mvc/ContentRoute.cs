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
	public class ContentRoute : RouteBase
	{
		public const string ContentItemKey = "ContentItem";
		public const string ContentEngineKey = "ContentEngine";
		const string ControllerKey = "controller";
		const string ActionKey = "action";
				
		IEngine engine;
		IRouteHandler routeHandler;
		IDictionary<Type, string> controllerMap = new Dictionary<Type, string>();

		public ContentRoute(IEngine engine, IRouteHandler routeHandler)
		{
			this.engine = engine;
			this.routeHandler = routeHandler;

			IList<ControlsAttribute> controllerDefinitions = FindControllers(engine);
			foreach (ItemDefinition id in engine.Definitions.GetDefinitions())
			{
				IControllerDescriptor controllerDefinition = GetControllerFor(id.ItemType, controllerDefinitions);
				controllerMap[id.ItemType] = controllerDefinition.ControllerName;
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
			ContentItem item = requestContext.RouteData.Values["ContentItem"] as ContentItem;

			if (item == null)
				return null;

			string action = "Index";

			if (values.ContainsKey("ContentItem"))
				item = (ContentItem)values["ContentItem"];
			if (values.ContainsKey("action"))
				action = (string)values["action"];

			Url url = item.Url;
			string defaultController = GetControllerName(item.GetType());
			string requestedController = defaultController;
			if (values.ContainsKey("controller"))
				requestedController = (string)values["controller"];

			if (string.Equals(defaultController, requestedController, StringComparison.InvariantCultureIgnoreCase))
			{
				return new VirtualPathData(this, url.AppendSegment(action).ToString().TrimStart('/'));
			}
			if (IsContentController(requestedController))
			{
				return new VirtualPathData(this, url.AppendSegment(action).ToString().TrimStart('/'));
			}
			return null;
		}

		private bool IsContentController(string controller)
		{
			return controllerMap.Any(kv => kv.Key.Name == controller);
		}

		IDictionary<Type, IList<IPathFinder>> FinderDictionary
		{
			get { return SingletonDictionary<Type, IList<IPathFinder>>.Instance; }
		}

		private string GetControllerName(Type type)
		{
			return controllerMap[type];
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
