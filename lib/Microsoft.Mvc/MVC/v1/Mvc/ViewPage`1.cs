namespace System.Web.Mvc {
    using System.Globalization;
    using System.Web.Resources;
    using System.Web.Mvc.Resources;

    [AspNetHostingPermission(System.Security.Permissions.SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    [AspNetHostingPermission(System.Security.Permissions.SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class ViewPage<TViewData> : ViewPage {
        private TViewData _viewData;

        public new TViewData ViewData {
            get {
                return _viewData;
            }
        }

        protected internal override void SetViewData(object viewData) {
            if (viewData != null && !(viewData is TViewData)) {
                throw new ArgumentException(
                    String.Format(
                        CultureInfo.CurrentUICulture,
                        MvcResources.ViewPageTViewData_WrongViewDataType,
                        viewData.GetType(),
                        typeof(TViewData)),
                    "viewData");
            }

            _viewData = (TViewData)viewData;
            base.SetViewData(viewData);
        }
    }
}
