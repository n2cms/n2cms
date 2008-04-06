namespace System.Web.Mvc {
    using System.Globalization;
    using System.Web.Resources;
    using System.Web.Mvc.Resources;

    [AspNetHostingPermission(System.Security.Permissions.SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    [AspNetHostingPermission(System.Security.Permissions.SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class ViewMasterPage<TViewData> : ViewMasterPage {
        public new TViewData ViewData {
            get {
                ViewPage<TViewData> viewPage = Page as ViewPage<TViewData>;
                if (viewPage == null) {
                    throw new InvalidOperationException(String.Format(CultureInfo.CurrentUICulture, MvcResources.ViewMasterPage_RequiresViewPageTViewData));
                }
                return viewPage.ViewData;
            }
        }
    }
}
