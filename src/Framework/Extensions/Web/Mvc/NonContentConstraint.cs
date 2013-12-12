using System.Web.Routing;

namespace N2.Web.Mvc
{
    /// <summary>
    /// Constraint used to prevent area routes from routing static routes for content.
    /// </summary>
    public class NonContentConstraint : IRouteConstraint
    {
        #region IRouteConstraint Members

        public bool Match(System.Web.HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            return values.ContainsKey(ContentRoute.ContentItemKey) == false;
        }

        #endregion
    }
}
