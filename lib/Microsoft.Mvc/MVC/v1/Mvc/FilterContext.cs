namespace System.Web.Mvc {
    using System;
    using System.Reflection;
    using System.Web;

    [AspNetHostingPermission(System.Security.Permissions.SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    [AspNetHostingPermission(System.Security.Permissions.SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class FilterContext : ControllerContext {

        public FilterContext(ControllerContext controllerContext, MethodInfo actionMethod)
            : base(controllerContext, GetControllerContext(controllerContext).Controller) {
            if (actionMethod == null) {
                throw new ArgumentNullException("actionMethod");
            }
            ActionMethod = actionMethod;
        }

        public MethodInfo ActionMethod {
            get;
            private set;
        }

        internal static ControllerContext GetControllerContext(ControllerContext controllerContext) {
            if (controllerContext == null) {
                throw new ArgumentNullException("controllerContext");
            }
            return controllerContext;
        }

        internal static FilterContext GetFilterContext(FilterContext filterContext) {
            if (filterContext == null) {
                throw new ArgumentNullException("filterContext");
            }
            return filterContext;
        }

    }
}
