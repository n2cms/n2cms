using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using N2.Engine;

namespace N2.Web.Mvc
{
    /// <summary>
    /// An ASP.NET MVC route that gets route data for content items of the specified generic type.
    /// </summary>
    /// <typeparam name="T">The type of content item to route.</typeparam>
    public class ContentRoute<T> : ContentRoute, IRouteWithArea
    {
        IEngine engine;

        public ContentRoute(IEngine engine)
            : base(engine)
        {
            this.engine = engine;
        }

        public ContentRoute(IEngine engine, IRouteHandler routeHandler, IControllerMapper controllerMapper, Route innerRoute)
            : base(engine, routeHandler, controllerMapper, innerRoute)
        {
            this.engine = engine;
            if (innerRoute.DataTokens.ContainsKey("area"))
                this.Area = innerRoute.DataTokens["area"] as string;
        }

        public override RouteValueDictionary GetRouteValues(ContentItem item, RouteValueDictionary routeValues)
        {
            if(item is T)
                return base.GetRouteValues(item, routeValues);
            return null;
        }

        public override RouteData GetRouteData(HttpContextBase httpContext)
        {
            var rd = base.GetRouteData(httpContext);

            if(rd != null && rd.CurrentItem() is T)
                // don't route items of the wrong type
                return rd;

            return null;
        }

        public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
        {
            var item = values.CurrentItem<ContentItem>(ContentItemKey, engine.Persister)
                ?? requestContext.CurrentItem();

            // only supply path to items of the correct type
            if (!(item is T))
                return null;

            values = new RouteValueDictionary(values);
            values[AreaKey] = Area;
            var vpd = base.GetVirtualPath(requestContext, values);

            if (vpd != null && Area != null)
                vpd.DataTokens[AreaKey] = Area;

            return vpd;
        }

        #region IRouteWithArea Members

        string area;
        public string Area
        {
            get { return area; }
            protected set { area = value; }
        }

        #endregion
    }
}
