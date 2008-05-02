using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;
using System.Web.Mvc;
using System.Web;
using N2.Engine;
using N2.Definitions;

namespace N2.Web.Mvc
{
	public class ContentRoute : RouteBase
	{
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
				controllerMap[id.ItemType] = GetControllerFor(id.ItemType, controllerDefinitions);
			}
		}

		public override RouteData GetRouteData(HttpContextBase httpContext)
		{
			string path = httpContext.Request.AppRelativeCurrentExecutionFilePath;
			if (path.StartsWith("~/edit/", StringComparison.InvariantCultureIgnoreCase))
				return null;
			else if (path.EndsWith(".axd", StringComparison.InvariantCultureIgnoreCase))
				return null;

			ContentItem item = engine.UrlParser.Parse(httpContext.Request.RawUrl);
			if (item != null)
			{
				RouteData data = new RouteData(this, routeHandler);

				data.Values["ContentItem"] = item;
				data.Values["ContentEngine"] = engine;
				data.Values["controller"] = GetControllerName(item.GetType());
				data.Values["action"] = httpContext.Request["action"] ?? "index"; //TODO: fix something neat
				return data;
			}
			else
			{
				return null;
			}
		}

		public override VirtualPathData GetVirtualPath(System.Web.Routing.RequestContext requestContext, RouteValueDictionary values)
		{
			ContentItem item = requestContext.RouteData.Values["ContentItem"] as ContentItem;
			string action = "Index";
			
			if (values.ContainsKey("ContentItem"))
				item = values["ContentItem"] as ContentItem;
			if (values.ContainsKey("action"))
				action = (string)values["action"];
			
			string url = item.Url;
			string defaultController = GetControllerName(item.GetType());
			string requestedController = (string)values["controller"];
			
			if (string.Equals(defaultController, requestedController, StringComparison.InvariantCultureIgnoreCase))
			{
				return new VirtualPathData(this, url + QuerySeparator(url) + "action=" + action);
			}
			else
			{
				return new VirtualPathData(this, url + QuerySeparator(url) + "controller=" + requestedController + "&action=" + action);
			}
		}

		private static string QuerySeparator(string url)
		{
			return url.Contains('?') ? "&" : "?";
		}

		private string GetControllerName(Type type)
		{
			return controllerMap[type];
		}

		private string GetControllerFor(Type itemType, IList<ControlsAttribute> controllerDefinitions)
		{
			foreach (ControlsAttribute controllerDefinition in controllerDefinitions)
			{
				if (controllerDefinition.ItemType.IsAssignableFrom(itemType))
				{
					string name = controllerDefinition.ControllerType.Name;
					int i = name.IndexOf("Controller");
					if (i > 0)
					{
						return name.Substring(0, i);
					}
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
