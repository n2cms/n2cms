using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using N2.Engine;

namespace N2.Web.Mvc
{
	public class AreaRegistrationState
	{
		public AreaRegistrationState(IEngine engine)
		{
			Engine = engine;
			Values = new Dictionary<string, object>();
		}

		public IEngine Engine { get; set; }
		public IDictionary<string, object> Values { get; set; }
	}

	public static class AreaRegistrationExtensions
	{
		/// <summary>Maps a content route to the route collection.</summary>
		/// <param name="routes">The route collection to extend.</param>
		/// <param name="name">The name of this route.</param>
		/// <param name="engine">The N2 Engine instance used by the content route.</param>
		/// <returns>The added content route instance.</returns>
		public static ContentRoute MapContentRoute<T>(this AreaRegistrationContext arc, string[] namespaces = null)
			where T: ContentItem
		{
			var state = arc.State as AreaRegistrationState;
			if (state == null) throw new ArgumentException("The area registration context didn't contain an AreaRegistrationState.", "arc");
			if (state.Engine == null) throw new ArgumentException("The area registration state an IEngine.", "arc");
			
			var routeHandler = new MvcRouteHandler();
			var controllerMapper = state.Engine.Resolve<IControllerMapper>();
			var innerRoute = new Route("{area}/{controller}/{action}",
				new RouteValueDictionary(new { action = "Index" }),
				new RouteValueDictionary(),
				new RouteValueDictionary(new { engine = state.Engine, area = arc.AreaName, namespaces }),
				routeHandler);

			var cr = new ContentRoute<T>(state.Engine, routeHandler, controllerMapper, innerRoute);
			arc.Routes.Add(arc.AreaName + "_" + typeof(T).FullName, cr);
			return cr;
		}
	}
}
