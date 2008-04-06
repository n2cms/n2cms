namespace System.Web.Mvc {
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Web.Hosting;
    using System.Web.Resources;
    using System.Web.Routing;
    using System.Web.Mvc.Resources;

    public class ViewLocator : IViewLocator {
        private VirtualPathProvider _vpp;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        public string[] MasterLocationFormats {
            get;
            set;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        public string[] ViewLocationFormats {
            get;
            set;
        }

        internal VirtualPathProvider VirtualPathProvider {
            get {
                if (_vpp == null) {
                    _vpp = HostingEnvironment.VirtualPathProvider;
                }
                return _vpp;
            }
            set {
                _vpp = value;
            }
        }

        private static string GetErrorLocations(string[] locationFormats, RequestContext requestContext, string name) {
            if (IsSpecificPath(name)) {
                return name;
            }

            List<string> paths = new List<string>();
            ProcessLocationFormats(locationFormats, requestContext, name, delegate(string path) {
                paths.Add(path);
                return false;
            });

            return String.Join(", ", paths.ToArray());
        }

        protected virtual string GetMasterLocation(RequestContext requestContext, string masterName) {
            if (requestContext == null) {
                throw new ArgumentNullException("requestContext");
            }

            string path = GetPath(requestContext, MasterLocationFormats, masterName);
            if (path == null) {
                throw new InvalidOperationException(
                    String.Format(
                        CultureInfo.CurrentUICulture,
                        MvcResources.ViewLocator_MasterNotFound,
                        masterName,
                        GetErrorLocations(MasterLocationFormats, requestContext, masterName)));
            }
            return path;
        }

        protected virtual string GetPath(RequestContext requestContext, string[] locationFormats, string name) {
            if (requestContext == null) {
                throw new ArgumentNullException("requestContext");
            }
            if (locationFormats == null || locationFormats.Length == 0) {
                throw new ArgumentException(MvcResources.ViewLocator_LocationsRequired, "locationFormats");
            }

            if (IsSpecificPath(name)) {
                // If the path is fully specified, just use it
                if (VirtualPathProvider.FileExists(name)) {
                    return name;
                }
            }
            else {
                // If the path is not fully specified, apply the format strings
                string foundViewLocation = null;

                ProcessLocationFormats(locationFormats, requestContext, name, delegate(string viewLocation) {
                    bool fileExists = VirtualPathProvider.FileExists(viewLocation);
                    if (fileExists) {
                        foundViewLocation = viewLocation;
                    }
                    return fileExists;
                });

                return foundViewLocation;
            }

            return null;
        }

        protected virtual string GetViewLocation(RequestContext requestContext, string viewName) {
            if (requestContext == null) {
                throw new ArgumentNullException("requestContext");
            }

            string path = GetPath(requestContext, ViewLocationFormats, viewName);
            if (path == null) {
                throw new InvalidOperationException(
                    String.Format(
                        CultureInfo.CurrentUICulture,
                        MvcResources.ViewLocator_ViewNotFound,
                        viewName,
                        GetErrorLocations(ViewLocationFormats, requestContext, viewName)));
            }
            return path;
        }

        private static bool IsSpecificPath(string name) {
            return 
                name.StartsWith("~", StringComparison.Ordinal) ||
                name.StartsWith("/", StringComparison.Ordinal);
        }

        private static void ProcessLocationFormats(string[] locationFormats, RequestContext requestContext, string name, Predicate<string> predicate) {
            string controllerName = requestContext.RouteData.GetRequiredString("controller");
            foreach (string locationFormat in locationFormats) {
                string viewLocation = String.Format(CultureInfo.InvariantCulture, locationFormat, name, controllerName);
                if (predicate(viewLocation)) {
                    return;
                }
            }
        }

        #region IViewLocator Members
        string IViewLocator.GetViewLocation(RequestContext requestContext, string viewName) {
            return GetViewLocation(requestContext, viewName);
        }

        string IViewLocator.GetMasterLocation(RequestContext requestContext, string masterName) {
            return GetMasterLocation(requestContext, masterName);
        }
        #endregion
    }
}
