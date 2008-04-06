namespace System.Web.Mvc {
    using System;
    using System.Web;

    [AspNetHostingPermission(System.Security.Permissions.SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    [AspNetHostingPermission(System.Security.Permissions.SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class FilterExecutedContext : FilterContext {

        public FilterExecutedContext(FilterContext filterContext, Exception exception)
            : base(filterContext, GetFilterContext(filterContext).ActionMethod) {
            Exception = exception;
        }

        public Exception Exception {
            get;
            private set;
        }

        public bool ExceptionHandled {
            get;
            set;
        }

    }
}
