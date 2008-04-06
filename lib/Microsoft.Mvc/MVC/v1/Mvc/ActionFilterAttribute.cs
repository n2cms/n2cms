namespace System.Web.Mvc {
    using System;
    using System.Web;
    using System.Web.Mvc.Resources;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    [AspNetHostingPermission(System.Security.Permissions.SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    [AspNetHostingPermission(System.Security.Permissions.SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public abstract class ActionFilterAttribute : Attribute {

        private int _order = -1;

        public int Order {
            get {
                return _order;
            }
            set {
                if (value < -1) {
                    throw new ArgumentOutOfRangeException("value",
                        MvcResources.ActionFilter_OrderOutOfRange);
                }
                _order = value;
            }
        }

        // The OnXxx() methods are virtual rather than abstract so that a developer need override
        // only the ones that interest him.

        public virtual void OnActionExecuted(FilterExecutedContext filterContext) {
        }

        public virtual void OnActionExecuting(FilterExecutingContext filterContext) {
        }

    }

}
