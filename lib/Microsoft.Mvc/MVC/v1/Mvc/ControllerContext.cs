namespace System.Web.Mvc {
    using System.Web.Routing;

    [AspNetHostingPermission(System.Security.Permissions.SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    [AspNetHostingPermission(System.Security.Permissions.SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class ControllerContext : RequestContext {
        public ControllerContext(HttpContextBase httpContext, RouteData routeData, IController controller)
            : base(httpContext, routeData) {
            if (controller == null) {
                throw new ArgumentNullException("controller");
            }
            Controller = controller;
        }

        public ControllerContext(RequestContext requestContext, IController controller)
            : this(GetRequestContext(requestContext).HttpContext, GetRequestContext(requestContext).RouteData, controller) {
        }

        public IController Controller {
            get;
            private set;
        }

        internal static RequestContext GetRequestContext(RequestContext requestContext) {
            if (requestContext == null) {
                throw new ArgumentNullException("requestContext");
            }
            return requestContext;
        }
    }
}
