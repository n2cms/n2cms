using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;

namespace N2.Engine.Providers
{
    [Service(typeof(IProvider<RouteCollection>))]
    public class RouteCollectionProvider : IProvider<RouteCollection>
    {
        #region IProvider<RouteCollection> Members

        public RouteCollection Get()
        {
            return RouteTable.Routes;
        }

        public IEnumerable<RouteCollection> GetAll()
        {
            return new[] { RouteTable.Routes };
        }

        #endregion
    }
}
