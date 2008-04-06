namespace System.Web.Mvc {
    using System.Globalization;
    using System.Web.Resources;
    using System.Web.Mvc.Resources;

    [AspNetHostingPermission(System.Security.Permissions.SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    [AspNetHostingPermission(System.Security.Permissions.SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class ViewUserControl<TViewData> : ViewUserControl {
        private TViewData _viewData;

        public new TViewData ViewData {
            get {
                EnsureViewData();
                return _viewData;
            }
        }
        private void EnsureViewData() {
            // Get the ViewData for this ViewUserControl, optionally using the specified ViewDataKey
            if (_viewData != null) {
                return;
            }
            object viewData = ((IViewDataContainer)this).ViewData;
            if (viewData == null || !(viewData is TViewData)) {
                throw new InvalidOperationException(
                    String.Format(
                        CultureInfo.CurrentUICulture,
                        MvcResources.ViewUserControl_WrongTViewDataType,
                        AppRelativeVirtualPath,
                        typeof(TViewData)));
            }
            _viewData = (TViewData)viewData;
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
