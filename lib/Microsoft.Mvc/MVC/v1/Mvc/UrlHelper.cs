namespace System.Web.Mvc {
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Web.Mvc.Resources;
    using System.Web.Routing;

    [AspNetHostingPermission(System.Security.Permissions.SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    [AspNetHostingPermission(System.Security.Permissions.SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class UrlHelper {
        private RouteCollection _routeCollection;

        internal RouteCollection RouteCollection {
            get {
                if (_routeCollection == null) {
                    _routeCollection = RouteTable.Routes;
                }
                return _routeCollection;
            }
            set {
                _routeCollection = value;
            }
        }

        public ViewContext ViewContext {
            get;
            private set;
        }

        public UrlHelper(ViewContext viewContext) {
            if (viewContext == null) {
                throw new ArgumentNullException("viewContext");
            }
            ViewContext = viewContext;
        }

        public string Action(string actionName) {
            if (String.IsNullOrEmpty(actionName)) {
                throw new ArgumentException(MvcResources.Common_NullOrEmpty, "actionName");
            }
            return GenerateUrl(null /* routeName */, actionName, null /* controllerName */, new RouteValueDictionary());
        }

        public string Action(string actionName, object values) {
            if (String.IsNullOrEmpty(actionName)) {
                throw new ArgumentException(MvcResources.Common_NullOrEmpty, "actionName");
            }
            return GenerateUrl(null /* routeName */, actionName, null /* controllerName */, new RouteValueDictionary(values));
        }

        public string Action(string actionName, RouteValueDictionary valuesDictionary) {
            if (String.IsNullOrEmpty(actionName)) {
                throw new ArgumentException(MvcResources.Common_NullOrEmpty, "actionName");
            }
            if (valuesDictionary == null) {
                throw new ArgumentNullException("valuesDictionary");
            }
            return GenerateUrl(null /* routeName */, actionName, null /* controllerName */, new RouteValueDictionary(valuesDictionary));
        }

        public string Action(string actionName, string controllerName) {
            if (String.IsNullOrEmpty(actionName)) {
                throw new ArgumentException(MvcResources.Common_NullOrEmpty, "actionName");
            }
            if (String.IsNullOrEmpty(controllerName)) {
                throw new ArgumentException(MvcResources.Common_NullOrEmpty, "controllerName");
            }
            return GenerateUrl(null /* routeName */, actionName, controllerName, new RouteValueDictionary());
        }

        public string Action(string actionName, string controllerName, object values) {
            if (String.IsNullOrEmpty(actionName)) {
                throw new ArgumentException(MvcResources.Common_NullOrEmpty, "actionName");
            }
            if (String.IsNullOrEmpty(controllerName)) {
                throw new ArgumentException(MvcResources.Common_NullOrEmpty, "controllerName");
            }
            return GenerateUrl(null /* routeName */, actionName, controllerName, new RouteValueDictionary(values));
        }

        public string Action(string actionName, string controllerName, RouteValueDictionary valuesDictionary) {
            if (String.IsNullOrEmpty(actionName)) {
                throw new ArgumentException(MvcResources.Common_NullOrEmpty, "actionName");
            }
            if (String.IsNullOrEmpty(controllerName)) {
                throw new ArgumentException(MvcResources.Common_NullOrEmpty, "controllerName");
            }
            if (valuesDictionary == null) {
                throw new ArgumentNullException("valuesDictionary");
            }
            return GenerateUrl(null /* routeName */, actionName, controllerName, new RouteValueDictionary(valuesDictionary));
        }

        public string Content(string contentPath) {
            if (String.IsNullOrEmpty(contentPath)) {
                throw new ArgumentException(MvcResources.Common_NullOrEmpty, "contentPath");
            }
            string appRelative = VirtualPathUtility.IsAppRelative(contentPath) ? contentPath : "~/" + contentPath;
            return VirtualPathUtility.ToAbsolute(appRelative, ViewContext.HttpContext.Request.ApplicationPath);
        }

        //REVIEW: Should we have an overload that takes Uri?
        [SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings",
            Justification = "As the return value will used only for rendering, string return value is more appropriate.")]
        [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings",
            Justification = "Needs to take same parameters as HttpUtility.UrlEncode()")]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic",
            Justification = "For consistency, all helpers are instance methods.")]
        public string Encode(string url) {
            return HttpUtility.UrlEncode(url);
        }

        private string GenerateUrl(string routeName, string actionName, string controllerName, RouteValueDictionary valuesDictionary) {
            return GenerateUrl(routeName, actionName, controllerName, valuesDictionary, RouteCollection, ViewContext);
        }

        internal static string GenerateUrl(string routeName, string actionName, string controllerName, RouteValueDictionary valuesDictionary, RouteCollection routeCollection, ViewContext viewContext) {
            if (actionName != null) {
                if (valuesDictionary.ContainsKey("action")) {
                    throw new ArgumentException(
                        String.Format(
                            CultureInfo.CurrentUICulture,
                            MvcResources.Helper_DictionaryAlreadyContainsKey,
                            "action"),
                        "actionName");
                }
                valuesDictionary.Add("action", actionName);
            }
            if (controllerName != null) {
                if (valuesDictionary.ContainsKey("controller")) {
                    throw new ArgumentException(
                        String.Format(
                            CultureInfo.CurrentUICulture,
                            MvcResources.Helper_DictionaryAlreadyContainsKey,
                            "controller"),
                        "controllerName");
                }
                valuesDictionary.Add("controller", controllerName);
            }

            VirtualPathData vpd;
            if (routeName != null) {
                vpd = routeCollection.GetVirtualPath(viewContext, routeName, valuesDictionary);
            }
            else {
                vpd = routeCollection.GetVirtualPath(viewContext, valuesDictionary);
            }

            if (vpd != null) {
                return vpd.VirtualPath;
            }
            return null;
        }

        [SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings",
            Justification = "As the return value will used only for rendering, string return value is more appropriate.")]
        public string RouteUrl(object values) {
            return GenerateUrl(null /* routeName */, null /* actionName */, null /* controllerName */, new RouteValueDictionary(values));
        }

        [SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings",
            Justification = "As the return value will used only for rendering, string return value is more appropriate.")]
        public string RouteUrl(RouteValueDictionary valuesDictionary) {
            if (valuesDictionary == null) {
                throw new ArgumentNullException("valuesDictionary");
            }
            return GenerateUrl(null /* routeName */, null /* actionName */, null /* controllerName */, new RouteValueDictionary(valuesDictionary));
        }

        [SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings",
            Justification = "As the return value will used only for rendering, string return value is more appropriate.")]
        public string RouteUrl(string routeName) {
            if (String.IsNullOrEmpty(routeName)) {
                throw new ArgumentException(MvcResources.Common_NullOrEmpty, "routeName");
            }
            return GenerateUrl(routeName, null /* actionName */, null /* controllerName */, new RouteValueDictionary());
        }

        [SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings",
            Justification = "As the return value will used only for rendering, string return value is more appropriate.")]
        public string RouteUrl(string routeName, object values) {
            if (String.IsNullOrEmpty(routeName)) {
                throw new ArgumentException(MvcResources.Common_NullOrEmpty, "routeName");
            }
            return GenerateUrl(routeName, null /* actionName */, null /* controllerName */, new RouteValueDictionary(values));
        }

        [SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings",
            Justification = "As the return value will used only for rendering, string return value is more appropriate.")]
        public string RouteUrl(string routeName, RouteValueDictionary valuesDictionary) {
            if (String.IsNullOrEmpty(routeName)) {
                throw new ArgumentException(MvcResources.Common_NullOrEmpty, "routeName");
            }
            if (valuesDictionary == null) {
                throw new ArgumentNullException("valuesDictionary");
            }
            return GenerateUrl(routeName, null /* actionName */, null /* controllerName */, new RouteValueDictionary(valuesDictionary));
        }
    }
}
