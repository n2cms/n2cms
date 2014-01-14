using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using N2.Engine;

namespace N2.Web.Mvc
{
    /// <summary>
    /// A view engine that only forwards to the inner view engine when the route is the the allowed route.
    /// </summary>
    public class PrivateViewEngineDecorator : IViewEngine, IDecorator<IViewEngine>
    {
        IViewEngine inner;
        RouteBase allwedRoute;

        public PrivateViewEngineDecorator(IViewEngine inner, RouteBase allwedRoute)
        {
            this.inner = inner;
            this.allwedRoute = allwedRoute;
        }

        #region IViewEngine Members

        public ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache)
        {
            if (IsAllowed(controllerContext))
                return inner.FindPartialView(controllerContext, partialViewName, useCache);

            return new ViewEngineResult(Enumerable.Empty<string>());
        }

        public ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
        {
            if (IsAllowed(controllerContext))
                return inner.FindView(controllerContext, viewName, masterName, useCache);

            return new ViewEngineResult(Enumerable.Empty<string>());
        }

        public void ReleaseView(ControllerContext controllerContext, IView view)
        {
            if (IsAllowed(controllerContext))
                inner.ReleaseView(controllerContext, view);
        }

        private bool IsAllowed(ControllerContext controllerContext)
        {
            return allwedRoute == controllerContext.RouteData.Route;
        }

        #endregion

        public IViewEngine Component
        {
            get { return inner; }
        }
    }
}
