namespace System.Web.Mvc {
    using System.Web.Routing;

    [AspNetHostingPermission(System.Security.Permissions.SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    [AspNetHostingPermission(System.Security.Permissions.SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class MvcRouteHandler : IRouteHandler {
        protected virtual IHttpHandler GetHttpHandler(RequestContext requestContext) {
            return new MvcHandler(requestContext);
        }

        #region IRouteHandler Members
        IHttpHandler IRouteHandler.GetHttpHandler(RequestContext requestContext) {
            return GetHttpHandler(requestContext);
        }
        #endregion
    }
}
