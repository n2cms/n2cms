namespace System.Web.Mvc {
    using System;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Web;
    using System.Web.Compilation;

    public class ComponentController {
        public ComponentController() {
        }

        public ComponentController(ViewContext context) {
            Context = context;
        }

        public string ComponentRoot {
            get;
            set;
        }

        public ViewContext Context {
            get;
            set;
        }

        public string RenderedHtml {
            get;
            set;
        }

        public void RenderView(string viewName) {
            RenderView(viewName, null);
        }

        public void RenderView(string viewName, object viewData) {
            if (!viewName.EndsWith(".aspx", StringComparison.OrdinalIgnoreCase))
                viewName += ".aspx";

            // TODO: Use better logic to chop off the controller suffix
            string controllerName = GetType().Name.Replace("Controller", "");

            // TODO: Use the ViewLocator logic to get the correct path
            //the path should stem from "Components"
            string componentRoot = "~/Components/" + controllerName;
            string viewPath = componentRoot + "/Views/" + viewName;

            Type pageType = BuildManager.GetCompiledType(viewPath);
            ViewPage pageInstance = (ViewPage)Activator.CreateInstance(pageType);

            //set the Url and Html
            pageInstance.Url = new UrlHelper(Context);
            pageInstance.Html = new HtmlHelper(Context);

            //set the ViewData
            pageInstance.SetViewData(viewData);

            //render
            StringBuilder sb = new StringBuilder();
            StringWriter writer = new StringWriter(sb, CultureInfo.CurrentCulture);

            HttpContext.Current.Server.Execute(pageInstance, writer, true);

            string pageText = sb.ToString();
            RenderedHtml = pageText;
        }
    }
}
