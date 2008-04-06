namespace System.Web.Mvc {
    using System.Web.UI;

    [AspNetHostingPermission(System.Security.Permissions.SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    [AspNetHostingPermission(System.Security.Permissions.SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class ViewPage : Page, IViewDataContainer {
        private string _masterLocation;
        private object _viewData;

        public string MasterLocation {
            get {
                return _masterLocation ?? String.Empty;
            }
            set {
                _masterLocation = value;
            }
        }

        public AjaxHelper Ajax {
            get;
            set;
        }

        public HtmlHelper Html {
            get;
            set;
        }

        public TempDataDictionary TempData {
            get {
                return ViewContext.TempData;
            }
        }

        public UrlHelper Url {
            get;
            set;
        }

        public ViewContext ViewContext {
            get;
            private set;
        }

        public ViewData ViewData {
            get {
                return new ViewData(_viewData);
            }
        }

        public HtmlTextWriter Writer {
            get;
            private set;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers")]
        protected override void OnPreInit(EventArgs e) {
            base.OnPreInit(e);

            if (!String.IsNullOrEmpty(MasterLocation)) {
                MasterPageFile = MasterLocation;
            }
        }

        public virtual void RenderView(ViewContext viewContext) {
            ViewContext = viewContext;
            Ajax = new AjaxHelper(viewContext);
            Html = new HtmlHelper(viewContext);
            Url = new UrlHelper(viewContext);

            ProcessRequest(HttpContext.Current);
        }

        protected override void Render(HtmlTextWriter writer) {
            Writer = writer;
            try {
                base.Render(writer);
            }
            finally {
                Writer = null;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate",
            Justification = "There is already a ViewData property and it has a slightly different meaning.")]
        protected internal virtual void SetViewData(object viewData) {
            _viewData = viewData;
        }

        #region IViewDataContainer Members
        object IViewDataContainer.ViewData {
            get {
                return _viewData;
            }
        }
        #endregion
    }
}
