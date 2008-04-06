namespace System.Web.Mvc {
    using System;
    using System.Collections;
    using System.Web;
    using System.Web.Compilation;
    using System.Web.Routing;

    public static class UserControlExtensions {
        /// <summary>
        /// Renders the specified ViewUserControl to a string
        /// </summary>
        /// <param name="virtualPath">The virtual path to the control</param>
        /// <returns>System.String</returns>
        public static string RenderUserControl(this ViewPage viewPage, string virtualPath) {
            return RenderUserControl(viewPage, virtualPath, null);
        }

        /// <summary>
        /// Renders the specified ViewUserControl to a string
        /// </summary>
        /// <param name="virtualPath">The virtual path to the control</param>
        /// <param name="controlData">The data to send to the control as ViewData</param>
        /// <returns>System.String</returns>
        public static string RenderUserControl(this ViewPage viewPage, string virtualPath, object controlData) {
            virtualPath = VirtualPathUtility.Combine(viewPage.AppRelativeTemplateSourceDirectory, virtualPath);
            return viewPage.Html.RenderUserControl(virtualPath, controlData, null);
        }

        /// <summary>
        /// Renders the specified ViewUserControl to a string
        /// </summary>
        /// <param name="virtualPath">The virtual path to the control</param>
        /// <param name="controlData">The data to send to the control as ViewData</param>
        /// <param name="propertySettings">Property settings for the control. Use Anonymous Typing for this: new{Name="MVC"}</param>
        /// <returns>System.String</returns>
        public static string RenderUserControl(this ViewPage viewPage, string virtualPath, object controlData, object propertySettings) {
            virtualPath = VirtualPathUtility.Combine(viewPage.AppRelativeTemplateSourceDirectory, virtualPath);
            return viewPage.Html.RenderUserControl(virtualPath, controlData, propertySettings);
        }

        /// <summary>
        /// Instantiates a ViewUserControl located at the supplied virtual path
        /// </summary>
        /// <param name="virtualPath">the virtual path to the control</param>
        /// <returns>ViewUserControl</returns>
        private static ViewUserControl InstantiateControl(string virtualPath) {
            ViewUserControl controlInstance = null;

            try {
                //check for the control, using the path as the key
                Type ctrlType = BuildManager.GetCompiledType(virtualPath);
                controlInstance = (ViewUserControl)Activator.CreateInstance(ctrlType);
            }
            catch (Exception x) {
                throw new InvalidOperationException("Unable to instantiate the ViewUserControl: " + x.Message, x);
            }
            return controlInstance;
        }

        /// <summary>
        /// Sets any properties on the control
        /// </summary>
        /// <param name="instance">The ViewUserControl instance</param>
        /// <param name="propertySettings"></param>
        private static void SetUserControlProperties(ViewUserControl instance, object propertySettings) {
            //property setter bits
            try {
                if (propertySettings != null) {
                    RouteValueDictionary values = new RouteValueDictionary(propertySettings);
                    HtmlExtensionUtility.SetPropertiesFromDictionary(instance, values);
                }
            }
            catch (Exception x) {
                throw new InvalidOperationException("Error setting properties for the ViewUserControl: " + x.Message, x);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Type ViewUserControl is required for this method")]
        public static string RenderUserControl<TControl>(ViewContext context, object controlData, object propertySettings) where TControl : ViewUserControl, new() {

            //instantiates the control
            TControl instance = new TControl();
            return DoRendering(instance, context, controlData, propertySettings);
        }

        /// <summary>
        /// Renders the specified ViewUserControl to a string
        /// </summary>
        /// <param name="virtualPath">The virtual path to the control</param>
        /// <returns>System.String</returns>
        public static string RenderUserControl(this HtmlHelper helper, string virtualPath) {
            return RenderUserControl(helper, virtualPath, null, null);
        }

        public static string RenderUserControl(string virtualPath, ViewContext context) {
            ViewUserControl instance = InstantiateControl(virtualPath);
            return DoRendering(instance, context, null, null);
        }

        /// <summary>
        /// Renders the specified ViewUserControl to a string
        /// </summary>
        /// <param name="virtualPath">The virtual path to the control</param>
        /// <param name="controlData">The data to send to the control as ViewData</param>
        /// <returns>System.String</returns>
        public static string RenderUserControl(this HtmlHelper helper, string virtualPath, object controlData) {
            return RenderUserControl(helper, virtualPath, controlData, null);
        }

        /// <summary>
        /// Renders the specified ViewUserControl to a string
        /// </summary>
        /// <param name="virtualPath">The virtual path to the control</param>
        /// <param name="controlData">The data to send to the control as ViewData</param>
        /// <param name="propertySettings">Property settings for the control. Use Anonymous Typing for this: new{Name="MVC"}</param>
        /// <returns>System.String</returns>
        public static string RenderUserControl(this HtmlHelper helper, string virtualPath, object controlData, object propertySettings) {
            ViewUserControl instance = InstantiateControl(virtualPath);
            return DoRendering(instance, helper.ViewContext, controlData, propertySettings);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Type ViewUserControl is required for this method")]
        public static string RenderUserControl<TControl>(this HtmlHelper helper, object controlData, object propertySettings) where TControl : ViewUserControl, new() {

            //instantiates the control
            TControl instance = new TControl();
            return DoRendering(instance, helper.ViewContext, controlData, propertySettings);
        }

        private static string DoRendering(ViewUserControl instance, ViewContext context, object controlData, object propertySettings) {
            ViewPage dummyPage = new ViewPage();
            dummyPage.Controls.Add(instance);

            //pass it the default context from the helper
            dummyPage.Url = new UrlHelper(context);
            dummyPage.Html = new HtmlHelper(context);

            //set the properties
            SetUserControlProperties(instance, propertySettings);

            if (controlData == null) {
                instance.SetViewData(context.ViewData);
            }
            else {
                instance.SetViewData(controlData);
            }

            //Render it
            string result = HtmlExtensionUtility.RenderPage(dummyPage);

            return result;
        }
    }
}
