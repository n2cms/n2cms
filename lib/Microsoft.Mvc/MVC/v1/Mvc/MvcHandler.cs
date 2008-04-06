namespace System.Web.Mvc {
    using System.Globalization;
    using System.Web.Mvc.Resources;
    using System.Web.Routing;
    using System.Web.SessionState;

    [AspNetHostingPermission(System.Security.Permissions.SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    [AspNetHostingPermission(System.Security.Permissions.SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class MvcHandler : IHttpHandler, IRequiresSessionState {
        private ControllerBuilder _controllerBuilder;

        public MvcHandler(RequestContext requestContext) {
            if (requestContext == null) {
                throw new ArgumentNullException("requestContext");
            }
            RequestContext = requestContext;
        }

        protected virtual bool IsReusable {
            get {
                // REVIEW: What's this?
                return false;
            }
        }

        internal ControllerBuilder ControllerBuilder {
            get {
                if (_controllerBuilder == null) {
                    _controllerBuilder = ControllerBuilder.Current;
                }
                return _controllerBuilder;
            }
            set {
                _controllerBuilder = value;
            }
        }

        public RequestContext RequestContext {
            get;
            private set;
        }

        protected virtual void ProcessRequest(HttpContext httpContext) {
            HttpContextBase iHttpContext = new HttpContextWrapper2(httpContext);
            ProcessRequest(iHttpContext);
        }

        protected internal virtual void ProcessRequest(HttpContextBase httpContext) {
            // Get the controller type
            string controllerName = RequestContext.RouteData.GetRequiredString("controller");

            // Instantiate the controller and call Execute
            IControllerFactory factory = ControllerBuilder.GetControllerFactory();
            IController controller = factory.CreateController(RequestContext, controllerName);
            if (controller == null) {
                throw new InvalidOperationException(
                    String.Format(
                        CultureInfo.CurrentUICulture,
                        MvcResources.ControllerBuilder_FactoryReturnedNull,
                        factory.GetType(),
                        controllerName));
            }
            try {
                ControllerContext controllerContext = new ControllerContext(RequestContext, controller);
                controller.Execute(controllerContext);
            }
            finally {
                factory.DisposeController(controller);
            }
        }

        #region IHttpHandler Members
        bool IHttpHandler.IsReusable {
            get {
                return IsReusable;
            }
        }

        void IHttpHandler.ProcessRequest(HttpContext httpContext) {
            ProcessRequest(httpContext);
        }
        #endregion
    }
}
