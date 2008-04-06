namespace System.Web.Mvc {
    using System;
    using System.Web;

    [AspNetHostingPermission(System.Security.Permissions.SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    [AspNetHostingPermission(System.Security.Permissions.SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class FilterExecutingContext : FilterContext {

        public FilterExecutingContext(FilterContext filterContext)
            : base(filterContext, GetFilterContext(filterContext).ActionMethod) {
        }

        public bool Cancel {
            get;
            set;
        }

    }
}
