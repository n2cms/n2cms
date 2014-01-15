using System.Collections.Generic;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web;
using System.Web.Routing;
using N2.Web;
using N2.Web.Mvc;

namespace N2.Engine.Providers
{
    [Service(typeof(IProvider<HtmlHelper>))]
    public class HtmlHelperProvider : IProvider<HtmlHelper>
    {
        IWebContext webContext;
        IProvider<RouteCollection> routeCollectionProvider;

        public HtmlHelperProvider(IWebContext webContext, IProvider<RouteCollection> routeCollectionProvider)
        {
            this.webContext = webContext;
            this.routeCollectionProvider = routeCollectionProvider;
        }

        #region IProvider<HtmlHelper> Members

        public HtmlHelper Get()
        {
            var httpContext = webContext.HttpContext;
            if (httpContext == null)
                return null;

            var routeData = GetRouteData();
            var cc = new ControllerContext() { HttpContext = httpContext, RequestContext = new RequestContext(httpContext, routeData), RouteData = routeData };
            return new HtmlHelper(
                new ViewContext(
                    cc,
                    new WebFormView(cc, httpContext.Request.AppRelativeCurrentExecutionFilePath),
                    new ViewDataDictionary(),
                    new TempDataDictionary(),
                    httpContext.Response.Output),
                new ViewPage(),
                routeCollectionProvider.Get());
        }

        private RouteData GetRouteData()
        {
            var routeData = new RouteData();
            RouteExtensions.ApplyCurrentPath(routeData, "WebForms", "Index", webContext.CurrentPath);
            return routeData;
        }

        public IEnumerable<HtmlHelper> GetAll()
        {
            return new[] { Get() };
        }

        #endregion
    }
}
