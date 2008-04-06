namespace System.Web.Mvc {
    using System.Globalization;
    using System.Web.Resources;
    using System.Web.UI;
    using System.Web.Mvc.Resources;

    [AspNetHostingPermission(System.Security.Permissions.SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    [AspNetHostingPermission(System.Security.Permissions.SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class ViewMasterPage : MasterPage {
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

        public ViewContext ViewContext {
            get {
                return ViewPage.ViewContext;
            }
        }

        public ViewData ViewData {
            get {
                return ViewPage.ViewData;
            }
        }

        private ViewPage ViewPage {
            get {
                ViewPage viewPage = Page as ViewPage;
                if (viewPage == null) {
                    throw new InvalidOperationException(String.Format(CultureInfo.CurrentUICulture, MvcResources.ViewMasterPage_RequiresViewPage));
                }
                return viewPage;
            }
        }

        public HtmlTextWriter Writer {
            get {
                return ViewPage.Writer;
            }
        }
    }
}
