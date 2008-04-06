namespace System.Web.Mvc {
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Web.Mvc.Resources;
    using System.Web.Routing;

    [AspNetHostingPermission(System.Security.Permissions.SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    [AspNetHostingPermission(System.Security.Permissions.SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class HtmlHelper {
        private const string _anchorTag = @"<a href=""{0}"">{1}</a>";
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

        public HtmlHelper(ViewContext viewContext) {
            if (viewContext == null) {
                throw new ArgumentNullException("viewContext");
            }
            ViewContext = viewContext;
        }

        public string ActionLink(string linkText, string actionName) {
            if (String.IsNullOrEmpty(linkText)) {
                throw new ArgumentException(MvcResources.Common_NullOrEmpty, "linkText");
            }
            if (String.IsNullOrEmpty(actionName)) {
                throw new ArgumentException(MvcResources.Common_NullOrEmpty, "actionName");
            }
            return GenerateLink(linkText, null /* routeName */, actionName, null /* controllerName */, new RouteValueDictionary());
        }

        public string ActionLink(string linkText, string actionName, object values) {
            if (String.IsNullOrEmpty(linkText)) {
                throw new ArgumentException(MvcResources.Common_NullOrEmpty, "linkText");
            }
            if (String.IsNullOrEmpty(actionName)) {
                throw new ArgumentException(MvcResources.Common_NullOrEmpty, "actionName");
            }
            return GenerateLink(linkText, null /* routeName */, actionName, null /* controllerName */, new RouteValueDictionary(values));
        }

        public string ActionLink(string linkText, string actionName, RouteValueDictionary valuesDictionary) {
            if (String.IsNullOrEmpty(linkText)) {
                throw new ArgumentException(MvcResources.Common_NullOrEmpty, "linkText");
            }
            if (String.IsNullOrEmpty(actionName)) {
                throw new ArgumentException(MvcResources.Common_NullOrEmpty, "actionName");
            }
            if (valuesDictionary == null) {
                throw new ArgumentNullException("valuesDictionary");
            }
            return GenerateLink(linkText, null /* routeName */, actionName, null /* controllerName */, new RouteValueDictionary(valuesDictionary));
        }

        public string ActionLink(string linkText, string actionName, string controllerName) {
            if (String.IsNullOrEmpty(linkText)) {
                throw new ArgumentException(MvcResources.Common_NullOrEmpty, "linkText");
            }
            if (String.IsNullOrEmpty(actionName)) {
                throw new ArgumentException(MvcResources.Common_NullOrEmpty, "actionName");
            }
            if (String.IsNullOrEmpty(controllerName)) {
                throw new ArgumentException(MvcResources.Common_NullOrEmpty, "controllerName");
            }
            return GenerateLink(linkText, null /* routeName */, actionName, controllerName, new RouteValueDictionary());
        }

        public string ActionLink(string linkText, string actionName, string controllerName, object values) {
            if (String.IsNullOrEmpty(linkText)) {
                throw new ArgumentException(MvcResources.Common_NullOrEmpty, "linkText");
            }
            if (String.IsNullOrEmpty(actionName)) {
                throw new ArgumentException(MvcResources.Common_NullOrEmpty, "actionName");
            }
            if (String.IsNullOrEmpty(controllerName)) {
                throw new ArgumentException(MvcResources.Common_NullOrEmpty, "controllerName");
            }
            return GenerateLink(linkText, null /* routeName */, actionName, controllerName, new RouteValueDictionary(values));
        }

        public string ActionLink(string linkText, string actionName, string controllerName, RouteValueDictionary valuesDictionary) {
            if (String.IsNullOrEmpty(linkText)) {
                throw new ArgumentException(MvcResources.Common_NullOrEmpty, "linkText");
            }
            if (String.IsNullOrEmpty(actionName)) {
                throw new ArgumentException(MvcResources.Common_NullOrEmpty, "actionName");
            }
            if (String.IsNullOrEmpty(controllerName)) {
                throw new ArgumentException(MvcResources.Common_NullOrEmpty, "controllerName");
            }
            if (valuesDictionary == null) {
                throw new ArgumentNullException("valuesDictionary");
            }
            return GenerateLink(linkText, null /* routeName */, actionName, controllerName, new RouteValueDictionary(valuesDictionary));
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic",
            Justification = "For consistency, all helpers are instance methods.")]
        public string Encode(string html) {
            return HttpUtility.HtmlEncode(html);
        }

        private string GenerateLink(string linkText, string routeName, string actionName, string controllerName, RouteValueDictionary valuesDictionary) {
            string url = UrlHelper.GenerateUrl(routeName, actionName, controllerName, valuesDictionary, RouteCollection, ViewContext);
            return String.Format(CultureInfo.InvariantCulture, _anchorTag, HttpUtility.HtmlAttributeEncode(url), HttpUtility.HtmlEncode(linkText));
        }

        public string RouteLink(string linkText, object values) {
            if (String.IsNullOrEmpty(linkText)) {
                throw new ArgumentException(MvcResources.Common_NullOrEmpty, "linkText");
            }
            return GenerateLink(linkText, null /* routeName */, null /* actionName */, null /* controllerName */, new RouteValueDictionary(values));
        }

        public string RouteLink(string linkText, RouteValueDictionary valuesDictionary) {
            if (String.IsNullOrEmpty(linkText)) {
                throw new ArgumentException(MvcResources.Common_NullOrEmpty, "linkText");
            }
            if (valuesDictionary == null) {
                throw new ArgumentNullException("valuesDictionary");
            }
            return GenerateLink(linkText, null /* routeName */, null /* actionName */, null /* controllerName */, new RouteValueDictionary(valuesDictionary));
        }

        public string RouteLink(string linkText, string routeName) {
            if (String.IsNullOrEmpty(linkText)) {
                throw new ArgumentException(MvcResources.Common_NullOrEmpty, "linkText");
            }
            if (String.IsNullOrEmpty(routeName)) {
                throw new ArgumentException(MvcResources.Common_NullOrEmpty, "routeName");
            }
            return GenerateLink(linkText, routeName, null /* actionName */, null /* controllerName */, new RouteValueDictionary());
        }

        public string RouteLink(string linkText, string routeName, object values) {
            if (String.IsNullOrEmpty(linkText)) {
                throw new ArgumentException(MvcResources.Common_NullOrEmpty, "linkText");
            }
            if (String.IsNullOrEmpty(routeName)) {
                throw new ArgumentException(MvcResources.Common_NullOrEmpty, "routeName");
            }
            return GenerateLink(linkText, routeName, null /* actionName */, null /* controllerName */, new RouteValueDictionary(values));
        }

        public string RouteLink(string linkText, string routeName, RouteValueDictionary valuesDictionary) {
            if (String.IsNullOrEmpty(linkText)) {
                throw new ArgumentException(MvcResources.Common_NullOrEmpty, "linkText");
            }
            if (String.IsNullOrEmpty(routeName)) {
                throw new ArgumentException(MvcResources.Common_NullOrEmpty, "routeName");
            }
            if (valuesDictionary == null) {
                throw new ArgumentNullException("valuesDictionary");
            }
            return GenerateLink(linkText, routeName, null /* actionName */, null /* controllerName */, new RouteValueDictionary(valuesDictionary));
        }
    }
}
