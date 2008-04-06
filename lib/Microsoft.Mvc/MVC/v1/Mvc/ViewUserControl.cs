namespace System.Web.Mvc {
    using System.ComponentModel;
    using System.Globalization;
    using System.Web.Resources;
    using System.Web.UI;
    using System.Web.Mvc.Resources;

    [AspNetHostingPermission(System.Security.Permissions.SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    [AspNetHostingPermission(System.Security.Permissions.SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class ViewUserControl : UserControl, IViewDataContainer {
        private string _viewDataKey;
        private object _viewData;

        public AjaxHelper Ajax {
            get {
                return ViewPage.Ajax;
            }
        }

        public HtmlHelper Html {
            get {
                return ViewPage.Html;
            }
        }

        public TempDataDictionary TempData {
            get {
                return ViewPage.TempData;
            }
        }

        public UrlHelper Url {
            get {
                return ViewPage.Url;
            }
        }

        public ViewData ViewData {
            get {
                EnsureViewData();
                return new ViewData(_viewData);
            }
        }

        public ViewContext ViewContext {
            get {
                return ViewPage.ViewContext;
            }
        }

        [DefaultValue("")]
        public string ViewDataKey {
            get {
                return _viewDataKey ?? String.Empty;
            }
            set {
                _viewDataKey = value;
            }
        }

        private ViewPage ViewPage {
            get {
                ViewPage viewPage = Page as ViewPage;
                if (viewPage == null) {
                    throw new InvalidOperationException(String.Format(CultureInfo.CurrentUICulture, MvcResources.ViewUserControl_RequiresViewPage));
                }
                return viewPage;
            }
        }

        public HtmlTextWriter Writer {
            get {
                return ViewPage.Writer;
            }
        }

        private void EnsureViewData() {
            // Get the ViewData for this ViewUserControl, optionally using the specified ViewDataKey
            if (_viewData != null) {
                return;
            }
            IViewDataContainer vdc = GetViewDataContainer(this);
            if (vdc == null) {
                throw new InvalidOperationException(
                    String.Format(
                        CultureInfo.CurrentUICulture,
                        MvcResources.ViewUserControl_RequiresViewDataProvider,
                        AppRelativeVirtualPath));
            }
            if (String.IsNullOrEmpty(ViewDataKey)) {
                _viewData = vdc.ViewData;
            }
            else {
                _viewData = DataBinder.Eval(vdc.ViewData, ViewDataKey);
            }
        }

        private static IViewDataContainer GetViewDataContainer(Control control) {
            // Walk up the control hierarchy until we find someone that implements IViewDataContainer
            while (control != null) {
                control = control.Parent;
                IViewDataContainer vdc = control as IViewDataContainer;
                if (vdc != null) {
                    return vdc;
                }
            }
            return null;
        }

        public virtual void RenderView(ViewContext viewContext) {
            // TODO: Remove this hack. Without it, the browser appears to always load cached output
            viewContext.HttpContext.Response.Cache.SetExpires(DateTime.Now);
            ViewUserControlContainerPage containerPage = new ViewUserControlContainerPage(this);
            containerPage.RenderView(viewContext);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate",
           Justification = "There is already a ViewData property and it has a slightly different meaning.")]
        protected internal virtual void SetViewData(object viewData) {
            _viewData = viewData;
        }

        #region IViewDataContainer Members
        object IViewDataContainer.ViewData {
            get {
                EnsureViewData();
                return _viewData;
            }
        }
        #endregion

        private sealed class ViewUserControlContainerPage : ViewPage {
            public ViewUserControlContainerPage(ViewUserControl userControl) {
                Controls.Add(userControl);
            }
        }
    }
}
