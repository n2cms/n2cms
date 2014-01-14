using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using N2.Engine;
using System.Collections.Generic;

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
        /// <param name="append">Append is useful in scenarios where you want to override the routing of specific urls also considered by N2.</param>
        /// <param name="append">Append is useful in scenarios where you want to override the routing of specific urls also considered by N2.</param>
        /// <param name="stopRewritableItems">Make the route table stop at items that match an item that can be rewritten to (probably webforms).</param>
        /// <param name="namespaces">The namespaces the routing engine will look for the controller in</param>
        /// <returns>The added content route instance.</returns>
        public static ContentRoute MapContentRoute(this RouteCollection routes, string name, IEngine engine, bool append = false, bool stopRewritableItems = true, string[] namespaces = null)
        {
			RemoveExistingRouteWithName(routes, name);

            var cr = new ContentRoute(engine, null, null, null, namespaces) { StopRewritableItems = stopRewritableItems };
            if (append)
            {
                routes.Add(cr);
                return cr;
            }

            var nonContentRoutes = SelectRoutesWithIndices(routes)
                .Where(x => !(x.Value is ContentRoute));
            int indexOfFirstNonContentRoute = nonContentRoutes.Any() 
                ? nonContentRoutes.Select(i => i.Key).FirstOrDefault() 
                : routes.Count;

            routes.Insert(indexOfFirstNonContentRoute, cr);
            return cr;
        }

		private static void RemoveExistingRouteWithName(RouteCollection routes, string name)
		{
			try
			{
				var existingRoute = routes[name];
				if (existingRoute != null)
					routes.Remove(existingRoute);
			}
			catch (Exception)
			{
			}
		}

        /// <summary>Maps a content route to the route collection.</summary>
        /// <param name="routes">The route collection to extend.</param>
        /// <param name="name">The name of this route.</param>
        /// <param name="engine">The N2 Engine instance used by the content route.</param>
        /// <param name="append">Append is useful in scenarios where you want to override the routing of specific urls also considered by N2.</param>
        /// <typeparam name="T">The type of content items to route.</typeparam>
        /// <returns>The added content route instance.</returns>
        public static ContentRoute MapContentRoute<T>(this RouteCollection routes, string name, IEngine engine, bool append = false)
        {
			RemoveExistingRouteWithName(routes, name);
			
			var cr = new ContentRoute<T>(engine);
            if (append)
            {
                routes.Add(cr);
                return cr;
            }

            var nonContentRoutesNorGenericRoutes = SelectRoutesWithIndices(routes)
                .Where(x => !(x.Value is ContentRoute) || !x.Value.GetType().ContainsGenericParameters);
            int indexOfFirstNonContentRoute = nonContentRoutesNorGenericRoutes.Any() 
                ? nonContentRoutesNorGenericRoutes.Select(i => i.Key).FirstOrDefault() 
                : routes.Count;

            routes.Insert(indexOfFirstNonContentRoute, cr);
            return cr;
        }

        private static IEnumerable<KeyValuePair<int, RouteBase>> SelectRoutesWithIndices(RouteCollection routes)
        {
            return routes.Select((rb, i) => new KeyValuePair<int, RouteBase>(i, rb));
        }
    }
}
