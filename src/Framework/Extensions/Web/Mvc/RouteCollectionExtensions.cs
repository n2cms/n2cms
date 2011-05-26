using System;
using System.Web.Mvc;
using System.Web.Routing;
using N2.Engine;

namespace N2.Web.Mvc
{
	/// <summary>
	/// Route registratoin extensions.
	/// </summary>
	public static class RouteCollectionExtensions
	{
		/// <summary>Maps a content route to the route collection.</summary>
		/// <param name="routes">The route collection to extend.</param>
		/// <param name="name">The name of this route.</param>
		/// <param name="engine">The N2 Engine instance used by the content route.</param>
		/// <returns>The added content route instance.</returns>
		public static ContentRoute MapContentRoute(this RouteCollection routes, string name, IEngine engine)
		{
			var cr = new ContentRoute(engine);
			routes.Insert(0, cr);
			return cr;
		}

		/// <summary>Maps a content route to the route collection.</summary>
		/// <param name="routes">The route collection to extend.</param>
		/// <param name="name">The name of this route.</param>
		/// <param name="engine">The N2 Engine instance used by the content route.</param>
		/// <param name="url">The url pattern used for this route (default is {controller}/{action}).</param>
		/// <param name="defaults">The default route values to use with this route.</param>
		/// <returns>The added content route instance.</returns>
		[Obsolete("TODO: support custom url")]
		private static ContentRoute MapContentRoute(this RouteCollection routes, string name, IEngine engine, string url, object defaults)
		{
			return routes.MapContentRoute(name, engine, url, defaults, null, null);
		}

		/// <summary>Maps a content route to the route collection.</summary>
		/// <param name="routes">The route collection to extend.</param>
		/// <param name="name">The name of this route.</param>
		/// <param name="engine">The N2 Engine instance used by the content route.</param>
		/// <param name="url">The url pattern used for this route (default is {controller}/{action}).</param>
		/// <param name="defaults">The default route values to use with this route.</param>
		/// <param name="constraints">A set of expressions that specify values for the url parameter.</param>
		/// <param name="namespaces">A set of namespaces for the application.</param>
		/// <returns>The added content route instance.</returns>
		[Obsolete("TODO: support custom url")]
		private static ContentRoute MapContentRoute(this RouteCollection routes, string name, IEngine engine, string url, object defaults, object constraints, string[] namespaces)
		{
			var rh = new MvcRouteHandler();
		    Route innerRoute = new Route(url, rh);
			innerRoute.Defaults = new RouteValueDictionary(defaults);
			innerRoute.Constraints = new RouteValueDictionary(constraints);
			innerRoute.DataTokens = new RouteValueDictionary();
			if ((namespaces != null) && (namespaces.Length > 0))
			{
				innerRoute.DataTokens["Namespaces"] = namespaces;
			}

			var cr = new ContentRoute(engine, rh, null, innerRoute);
			routes.Insert(0, cr);
			return cr;
		}



		/// <summary>Maps a content route to the route collection.</summary>
		/// <param name="routes">The route collection to extend.</param>
		/// <param name="name">The name of this route.</param>
		/// <param name="engine">The N2 Engine instance used by the content route.</param>
		/// <typeparam name="T">The type of content items to route.</typeparam>
		/// <returns>The added content route instance.</returns>
		public static ContentRoute MapContentRoute<T>(this RouteCollection routes, string name, IEngine engine)
			where T : ContentItem
		{
			var cr = new ContentRoute<T>(engine);
			routes.Insert(0, cr);
			return cr;
		}

		/// <summary>Maps a content route to the route collection.</summary>
		/// <param name="routes">The route collection to extend.</param>
		/// <param name="name">The name of this route.</param>
		/// <param name="engine">The N2 Engine instance used by the content route.</param>
		/// <param name="url">The url pattern used for this route (default is {controller}/{action}).</param>
		/// <param name="defaults">The default route values to use with this route.</param>
		/// <typeparam name="T">The type of content items to route.</typeparam>
		/// <returns>The added content route instance.</returns>
		[Obsolete("TODO: support custom url")]
		private static ContentRoute MapContentRoute<T>(this RouteCollection routes, string name, IEngine engine, string url, object defaults)
			where T : ContentItem
		{
			return routes.MapContentRoute<T>(name, engine, url, defaults, null, null);
		}

		/// <summary>Maps a content route to the route collection.</summary>
		/// <param name="routes">The route collection to extend.</param>
		/// <param name="name">The name of this route.</param>
		/// <param name="engine">The N2 Engine instance used by the content route.</param>
		/// <param name="url">The url pattern used for this route (default is {controller}/{action}).</param>
		/// <param name="defaults">The default route values to use with this route.</param>
		/// <param name="constraints">A set of expressions that specify values for the url parameter.</param>
		/// <param name="namespaces">A set of namespaces for the application.</param>
		/// <typeparam name="T">The type of content items to route.</typeparam>
		/// <returns>The added content route instance.</returns>
		[Obsolete("TODO: support custom url")]
		private static ContentRoute MapContentRoute<T>(this RouteCollection routes, string name, IEngine engine, string url, object defaults, object constraints, string[] namespaces)
				where T : ContentItem
		{
			var rh = new MvcRouteHandler();

			Route innerRoute = new Route(url, rh);
			innerRoute.Defaults = new RouteValueDictionary(defaults);
			innerRoute.Constraints = new RouteValueDictionary(constraints);
			innerRoute.DataTokens = new RouteValueDictionary();
			if ((namespaces != null) && (namespaces.Length > 0))
			{
				innerRoute.DataTokens["Namespaces"] = namespaces;
			}

			var cr = new ContentRoute<T>(engine, rh, null, innerRoute);
			routes.Insert(0, cr);
			return cr;
		}
	}
}
