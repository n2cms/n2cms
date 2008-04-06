namespace System.Web.Mvc {
    using System.Web.Routing;

    public interface IViewLocator {
        string GetViewLocation(RequestContext requestContext, string viewName);
        string GetMasterLocation(RequestContext requestContext, string masterName);
    }
}
