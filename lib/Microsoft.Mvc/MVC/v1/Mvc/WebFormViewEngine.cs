namespace System.Web.Mvc {
    using System;
    using System.Globalization;
    using System.Web.Compilation;
    using System.Web.Resources;
    using System.Web.Routing;
    using System.Web.Mvc.Resources;

    [AspNetHostingPermission(System.Security.Permissions.SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    [AspNetHostingPermission(System.Security.Permissions.SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class WebFormViewEngine : IViewEngine {
        private IBuildManager _buildManager;
        private IViewLocator _viewLocator;

        internal IBuildManager BuildManager {
            get {
                if (_buildManager == null) {
                    _buildManager = new BuildManagerWrapper();
                }
                return _buildManager;
            }
            set {
                _buildManager = value;
            }
        }

        public IViewLocator ViewLocator {
            get {
                if (_viewLocator == null) {
                    _viewLocator = new WebFormViewLocator();
                }
                return _viewLocator;
            }
            set {
                _viewLocator = value;
            }
        }

        protected virtual void RenderView(ViewContext viewContext) {
            if (viewContext == null) {
                throw new ArgumentNullException("viewContext");
            }

            string viewPath = ViewLocator.GetViewLocation(viewContext, viewContext.ViewName);
            if (String.IsNullOrEmpty(viewPath)) {
                throw new InvalidOperationException(
                    String.Format(
                        CultureInfo.CurrentUICulture,
                        MvcResources.WebFormViewEngine_ViewNotFound,
                        viewContext.ViewName));
            }
            object viewInstance = BuildManager.CreateInstanceFromVirtualPath(viewPath, typeof(object));
            if (viewInstance == null) {
                throw new InvalidOperationException(
                    String.Format(
                        CultureInfo.CurrentUICulture,
                        MvcResources.WebFormViewEngine_ViewCouldNotBeCreated,
                        viewPath));
            }

            ViewPage viewPage = viewInstance as ViewPage;

            if (viewPage != null) {

                if (!String.IsNullOrEmpty(viewContext.MasterName)) {
                    string masterLocation = ViewLocator.GetMasterLocation(viewContext, viewContext.MasterName);
                    if (String.IsNullOrEmpty(masterLocation)) {
                        throw new InvalidOperationException(
                            String.Format(
                                CultureInfo.CurrentUICulture,
                                MvcResources.WebFormViewEngine_MasterNotFound,
                                viewContext.MasterName));
                    }

                    // We don't set the page's MasterPageFile directly since it will get
                    // overwritten by a statically-defined value. In ViewPage we wait until
                    // the PreInit phase until we set the new value.
                    viewPage.MasterLocation = masterLocation;
                }
                viewPage.SetViewData(viewContext.ViewData);
                viewPage.RenderView(viewContext);
            }
            else {
                ViewUserControl viewUserControl = viewInstance as ViewUserControl;
                if (viewUserControl != null) {
                    if (!String.IsNullOrEmpty(viewContext.MasterName)) {
                        throw new InvalidOperationException(MvcResources.WebFormViewEngine_UserControlCannotHaveMaster);
                    }

                    viewUserControl.SetViewData(viewContext.ViewData);
                    viewUserControl.RenderView(viewContext);
                }
                else {
                    throw new InvalidOperationException(
                        String.Format(
                            CultureInfo.CurrentUICulture,
                            MvcResources.WebFormViewEngine_WrongViewBase,
                            viewPath));
                }
            }
        }

        #region IViewEngine Members
        void IViewEngine.RenderView(ViewContext viewContext) {
            RenderView(viewContext);
        }
        #endregion
    }
}
