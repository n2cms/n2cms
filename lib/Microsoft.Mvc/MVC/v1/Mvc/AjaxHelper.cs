namespace System.Web.Mvc {
    using System;

    [AspNetHostingPermission(System.Security.Permissions.SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    [AspNetHostingPermission(System.Security.Permissions.SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class AjaxHelper {
        public ViewContext ViewContext {
            get;
            private set;
        }

        public AjaxHelper(ViewContext viewContext) {
            if (viewContext == null) {
                throw new ArgumentNullException("viewContext");
            }
            ViewContext = viewContext;
        }
    }
}
