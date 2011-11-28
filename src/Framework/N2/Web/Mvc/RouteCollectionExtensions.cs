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
		/// <returns>The added content route instance.</returns>
		public static ContentRoute MapContentRoute(this RouteCollection routes, string name, IEngine engine)
		{
			var nonContentRoutes = SelectRoutesWithIndices(routes)
				.Where(x => !(x.Value is ContentRoute));
			int indexOfFirstNonContentRoute = nonContentRoutes.Any() 
				? nonContentRoutes.Select(i => i.Key).FirstOrDefault() 
				: routes.Count;

			var cr = new ContentRoute(engine);
			routes.Insert(indexOfFirstNonContentRoute, cr);
			return cr;
		}

		/// <summary>Maps a content route to the route collection.</summary>
		/// <param name="routes">The route collection to extend.</param>
		/// <param name="name">The name of this route.</param>
		/// <param name="engine">The N2 Engine instance used by the content route.</param>
		/// <typeparam name="T">The type of content items to route.</typeparam>
		/// <returns>The added content route instance.</returns>
		public static ContentRoute MapContentRoute<T>(this RouteCollection routes, string name, IEngine engine)
		{
			var nonContentRoutesNorGenericRoutes = SelectRoutesWithIndices(routes)
				.Where(x => !(x.Value is ContentRoute) || !x.Value.GetType().ContainsGenericParameters);
			int indexOfFirstNonContentRoute = nonContentRoutesNorGenericRoutes.Any() 
				? nonContentRoutesNorGenericRoutes.Select(i => i.Key).FirstOrDefault() 
				: routes.Count;

			var cr = new ContentRoute<T>(engine);
			routes.Insert(indexOfFirstNonContentRoute, cr);
			return cr;
		}

		private static IEnumerable<KeyValuePair<int, RouteBase>> SelectRoutesWithIndices(RouteCollection routes)
		{
			return routes.Select((rb, i) => new KeyValuePair<int, RouteBase>(i, rb));
		}
	}
}
