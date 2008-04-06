namespace System.Web.Mvc {
    using System;
    using System.Collections;
    using System.Globalization;
    using System.Reflection;
    using System.Web;
    using System.Web.Mvc.Resources;
    using System.Web.Routing;

    [AspNetHostingPermission(System.Security.Permissions.SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    [AspNetHostingPermission(System.Security.Permissions.SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class DefaultControllerFactory : IControllerFactory {
        private static object _controllerTypeCacheLock = new object();
        private static ControllerTypeCache _controllerTypeCache = new ControllerTypeCache();
        private ControllerTypeCache _currentControllerTypeCache;
        private IBuildManager _buildManager;

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

        internal ControllerTypeCache ControllerTypeCache {
            get {
                if (_currentControllerTypeCache == null) {
                    _currentControllerTypeCache = _controllerTypeCache;
                }
                return _currentControllerTypeCache;
            }
            set {
                _currentControllerTypeCache = value;
            }
        }

        public RequestContext RequestContext {
            get;
            set;
        }

        protected internal virtual IController CreateController(RequestContext requestContext, string controllerName) {
            if (requestContext == null) {
                throw new ArgumentNullException("requestContext");
            }
            if (String.IsNullOrEmpty(controllerName)) {
                throw new ArgumentException(MvcResources.Common_NullOrEmpty, "controllerName");
            }
            RequestContext = requestContext;
            Type controllerType = GetControllerType(controllerName);
            IController controller = GetControllerInstance(controllerType);
            return controller;
        }

        protected internal virtual void DisposeController(IController controller) {
            IDisposable disposable = controller as IDisposable;
            if (disposable != null) {
                disposable.Dispose();
            }
        }

        protected internal virtual IController GetControllerInstance(Type controllerType) {
            if (controllerType == null) {
                throw new ArgumentNullException(
                    "controllerType",
                    String.Format(
                        CultureInfo.CurrentUICulture,
                        MvcResources.DefaultControllerFactory_NoControllerFound,
                        RequestContext.HttpContext.Request.Path));
            }
            if (!typeof(IController).IsAssignableFrom(controllerType)) {
                throw new ArgumentException(
                    String.Format(
                        CultureInfo.CurrentUICulture,
                        MvcResources.DefaultControllerFactory_MissingIController,
                        controllerType),
                    "controllerType");
            }
            try {
                return Activator.CreateInstance(controllerType) as IController;
            }
            catch (Exception ex) {
                throw new InvalidOperationException(
                    String.Format(
                        CultureInfo.CurrentUICulture,
                        MvcResources.DefaultControllerFactory_ErrorCreatingController,
                        controllerType),
                    ex);
            }
        }

        protected internal virtual Type GetControllerType(string controllerName) {
            if (String.IsNullOrEmpty(controllerName)) {
                throw new ArgumentException(MvcResources.Common_NullOrEmpty, "controllerName");
            }

            if (!ControllerTypeCache.Initialized) {
                lock (_controllerTypeCacheLock) {
                    if (!ControllerTypeCache.Initialized) {
                        ControllerTypeCache.Initialize();
                        // Go through all assemblies referenced by the application and search for
                        // controllers and controller factories.
                        ICollection assemblies = BuildManager.GetReferencedAssemblies();
                        foreach (Assembly asm in assemblies) {
                            try {
                                Type[] typesInAsm = asm.GetTypes();
                                foreach (Type t in typesInAsm) {
                                    if (IsController(t)) {
                                        string foundControllerName = t.Name.Substring(0, t.Name.Length - 10);
                                        if (!ControllerTypeCache.ContainsController(foundControllerName)) {
                                            ControllerTypeCache.AddControllerType(foundControllerName, t);
                                        }
                                        else {
                                            // A null value indicates a conflict for this key (i.e. more than one match)
                                            ControllerTypeCache.AddControllerType(foundControllerName, null);
                                        }
                                    }
                                }
                            }
                            catch {
                                // TODO: Remove this try-catch. This is a temporary fix until we add a new feature.
                            }
                        }
                    }
                }
            }

            // Once the master list of controllers has been created we can quickly index into it
            Type controllerType;
            if (ControllerTypeCache.TryGetControllerType(controllerName, out controllerType)) {
                if (controllerType == null) {
                    // A null value indicates a conflict for this key (i.e. more than one match)
                    throw new InvalidOperationException(
                        String.Format(
                            CultureInfo.CurrentUICulture,
                            MvcResources.DefaultControllerFactory_DuplicateControllers,
                            controllerName));
                }
                return controllerType;
            }
            return null;
        }

        private static bool IsController(Type t) {
            return
                t.Name.EndsWith("Controller", StringComparison.OrdinalIgnoreCase) &&
                (t != typeof(IController)) &&
                (t != typeof(Controller)) &&
                typeof(IController).IsAssignableFrom(t);
        }

        #region IControllerFactory Members
        IController IControllerFactory.CreateController(RequestContext context, string controllerName) {
            return CreateController(context, controllerName);
        }

        void IControllerFactory.DisposeController(IController controller) {
            DisposeController(controller);
        }
        #endregion
    }
}
