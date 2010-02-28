using System.Web;
using System.Web.Routing;
using N2.Engine;

namespace N2.Web.Mvc
{
	/// <summary>
	/// An ASP.NET MVC route that gets route data for content items of the specified generic type.
	/// </summary>
	/// <typeparam name="T">The type of content item to route.</typeparam>
	public class ContentRoute<T> : ContentRoute where T:ContentItem
	{
		public ContentRoute(IEngine engine)
			: base(engine)
		{
		}

		public ContentRoute(IEngine engine, IRouteHandler routeHandler, IControllerMapper controllerMapper, Route innerRoute)
			: base(engine, routeHandler, controllerMapper, innerRoute)
		{
		}

		public override RouteData GetRouteData(HttpContextBase httpContext)
		{
			var rd = base.GetRouteData(httpContext);

			if(rd.DataTokens.CurrentItem() is T)
				// don't route items of the wrong type
				return rd;

			return null;
		}

		public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
		{
			if (values.CurrentItem() is T)
				// only supply path to items of the correct type
				return base.GetVirtualPath(requestContext, values);
			
			return null;
		}
	}
}
