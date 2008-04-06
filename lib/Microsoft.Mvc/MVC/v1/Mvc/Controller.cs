namespace System.Web.Mvc {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Reflection;
    using System.Security.Principal;
    using System.Web;
    using System.Web.Mvc.Resources;
    using System.Web.Routing;

    [AspNetHostingPermission(System.Security.Permissions.SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    [AspNetHostingPermission(System.Security.Permissions.SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class Controller : IController {
        private RouteCollection _routeCollection;
        private IDictionary<string, object> _viewData;
        private IViewEngine _viewEngine;

        public ControllerContext ControllerContext {
            get;
            set;
        }

        public HttpContextBase HttpContext {
            get {
                return ControllerContext == null ? null : ControllerContext.HttpContext;
            }
        }

        public HttpRequestBase Request {
            get {
                return HttpContext == null ? null : HttpContext.Request;
            }
        }

        public HttpResponseBase Response {
            get {
                return HttpContext == null ? null : HttpContext.Response;
            }
        }

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

        public RouteData RouteData {
            get {
                return ControllerContext == null ? null : ControllerContext.RouteData;
            }
        }

        public HttpServerUtilityBase Server {
            get {
                return HttpContext == null ? null : HttpContext.Server;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly",
            Justification = "This property is settable so that unit tests can provide mock implementations.")]
        public TempDataDictionary TempData {
            get;
            set;
        }

        public IPrincipal User {
            get {
                return HttpContext == null ? null : HttpContext.User;
            }
        }

        public IDictionary<string, object> ViewData {
            get {
                if (_viewData == null) {
                    _viewData = new Dictionary<string, object>();
                }
                return _viewData;
            }
        }

        public IViewEngine ViewEngine {
            get {
                return _viewEngine ?? new WebFormViewEngine();
            }
            set {
                if (value == null) {
                    throw new ArgumentNullException("value");
                }
                _viewEngine = value;
            }
        }

        private static object ConvertParameterType(object value, Type destinationType, string parameterName, string actionName) {
            if (value == null || value.GetType() == destinationType) {
                return value;
            }

            TypeConverter converter = TypeDescriptor.GetConverter(destinationType);
            bool canConvertFrom = converter.CanConvertFrom(value.GetType());
            if (!canConvertFrom) {
                converter = TypeDescriptor.GetConverter(value.GetType());
            }
            if (!(canConvertFrom || converter.CanConvertTo(destinationType))) {
                throw new InvalidOperationException(String.Format(
                    CultureInfo.CurrentUICulture,
                    MvcResources.Controller_CannotConvertParameter,
                    parameterName, actionName, value, destinationType));
            }
            try {
                return canConvertFrom ? converter.ConvertFrom(value) : converter.ConvertTo(value, destinationType);
            }
            catch (Exception ex) {
                throw new InvalidOperationException(String.Format(
                    CultureInfo.CurrentUICulture,
                    MvcResources.Controller_CannotConvertParameter,
                    parameterName, actionName, value, destinationType),
                    ex);
            }
        }

        protected internal virtual void Execute(ControllerContext controllerContext) {
            if (controllerContext == null) {
                throw new ArgumentNullException("controllerContext");
            }

            ControllerContext = controllerContext;
            TempData = new TempDataDictionary(controllerContext.HttpContext);

            string actionName = RouteData.GetRequiredString("action");
            if (!InvokeAction(actionName)) {
                HandleUnknownAction(actionName);
            }
        }

        protected internal virtual void HandleUnknownAction(string actionName) {
            throw new InvalidOperationException(String.Format(CultureInfo.CurrentUICulture, MvcResources.Controller_UnknownAction, actionName));
        }

        protected internal bool InvokeAction(string actionName) {
            return InvokeAction(actionName, new RouteValueDictionary());
        }

        protected internal virtual bool InvokeAction(string actionName, RouteValueDictionary values) {
            if (String.IsNullOrEmpty(actionName)) {
                throw new ArgumentException(MvcResources.Common_NullOrEmpty, "actionName");
            }

            // We have to loop through all the methods to make sure there isn't
            // a conflict. If we stop the loop the first time we find a match
            // we might miss some error cases.

            MemberInfo[] membInfos = GetType().GetMember(actionName, MemberTypes.Method,
                BindingFlags.IgnoreCase | BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Public);
            MethodInfo foundMatch = null;

            foreach (MemberInfo memberInfo in membInfos) {
                MethodInfo mi = (MethodInfo)memberInfo;

                // 1) Action methods must not have the non-action attribute in their inheritance chain, and
                // 2) special methods like constructors, property accessors, and event accessors cannot be action methods, and
                // 3) methods originally defined on Object (like ToString) or Controller cannot be action methods.
                if (!mi.IsDefined(typeof(NonActionAttribute), true) &&
                    !mi.IsSpecialName &&
                    mi.DeclaringType.IsSubclassOf(typeof(Controller))) {
                    if (foundMatch != null) {
                        throw new InvalidOperationException(
                            String.Format(CultureInfo.CurrentUICulture, MvcResources.Controller_MoreThanOneAction, actionName, GetType()));
                    }
                    foundMatch = mi;
                }
            }

            if (foundMatch != null) {
                InvokeActionMethod(foundMatch, values);
                return true;
            }
            return false;
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
            Justification = "We use MethodInfo since it represents only methods and not constructors." +
            "This method only makes sense for use with methods.")]
        protected internal virtual void InvokeActionMethod(MethodInfo methodInfo, RouteValueDictionary values) {
            if (methodInfo == null) {
                throw new ArgumentNullException("methodInfo");
            }
            if (values == null) {
                values = new RouteValueDictionary();
            }
            if (methodInfo.ContainsGenericParameters) {
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentUICulture, MvcResources.Controller_ActionCannotBeGeneric, methodInfo.Name));
            }

            ParameterInfo[] methodParameters = methodInfo.GetParameters();
            object[] parameterValues = null;
            if (methodParameters.Length > 0) {

                parameterValues = new object[methodParameters.Length];
                for (int i = 0; i < methodParameters.Length; i++) {
                    ParameterInfo pi = methodParameters[i];

                    if (pi.IsOut || pi.ParameterType.IsByRef) {
                        throw new InvalidOperationException(String.Format(CultureInfo.CurrentUICulture, MvcResources.Controller_ReferenceParametersNotSupported, pi.Name, methodInfo.Name));
                    }

                    bool valueRequired = true;
                    if (pi.ParameterType.IsClass) {
                        // Classes (ref types) don't require values since we can pass in null
                        valueRequired = false;
                    }
                    else {
                        if ((pi.ParameterType.IsGenericType && !pi.ParameterType.IsGenericTypeDefinition) &&
                            (pi.ParameterType.GetGenericTypeDefinition() == typeof(Nullable<>))) {
                            // Nullable types don't require values since we can pass in null
                            valueRequired = false;
                        }
                    }

                    // Try to get a value for the parameter. We use this order of precedence:
                    // 1. Explicitly-provided extra parameters in the call to InvokeAction()
                    // 2. Values from the RouteData (could be from the typed-in URL or from the route's default values)
                    // 3. Request values (query string, form post data, cookie)
                    object parameterValue = null;
                    if (!values.TryGetValue(methodParameters[i].Name, out parameterValue)) {
                        if (RouteData == null || !RouteData.Values.TryGetValue(methodParameters[i].Name, out parameterValue)) {
                            if (Request != null) {
                                parameterValue = Request[methodParameters[i].Name];
                            }
                        }
                    }

                    if (parameterValue == null && valueRequired) {
                        throw new InvalidOperationException(String.Format(CultureInfo.CurrentUICulture, MvcResources.Controller_MissingParameter, pi.Name, methodInfo.Name));
                    }

                    try {
                        parameterValues[i] = ConvertParameterType(parameterValue, methodParameters[i].ParameterType, methodParameters[i].Name, methodInfo.Name);
                    }
                    catch (Exception ex) {
                        // Parameter value conversion errors are acceptable unless the value is required
                        if (valueRequired) {
                            throw new InvalidOperationException(String.Format(CultureInfo.CurrentUICulture, MvcResources.Controller_MissingParameter, pi.Name, methodInfo.Name), ex);
                        }
                    }
                }
            }

            InvokeActionMethodFilters(methodInfo, () => methodInfo.Invoke(this, parameterValues));
        }

        private void InvokeActionMethodFilters(MethodInfo methodInfo, Action continuation) {

            // filters should execute in this order:
            // controller virtual overrides -> controller base attributes -> controller type attributes ->
            //   base action method attributes -> action method attributes

            List<ActionFilterAttribute> filters = new List<ActionFilterAttribute>() {
                new ControllerActionFilter(this)
            };

            Stack<MemberInfo> memberChain = new Stack<MemberInfo>();
            Type curType = GetType();
            while (curType != null) {
                memberChain.Push(curType);
                curType = curType.BaseType;
            }

            List<ActionFilterAttribute> sortedClassFilters = SortActionFilters(memberChain);
            filters.AddRange(sortedClassFilters);
            List<ActionFilterAttribute> sortedMethodFilters = PrepareMethodActionFilters(methodInfo);
            filters.AddRange(sortedMethodFilters);

            FilterContext context = new FilterContext(ControllerContext, methodInfo);
            ActionFilterExecutor executor = new ActionFilterExecutor(filters, context, continuation);
            executor.Execute();
        }

        protected virtual void OnActionExecuted(FilterExecutedContext filterContext) {
        }

        protected virtual void OnActionExecuting(FilterExecutingContext filterContext) {
        }

        internal static List<ActionFilterAttribute> PrepareMethodActionFilters(MethodInfo methodInfo) {

            Stack<MemberInfo> memberChain = new Stack<MemberInfo>();
            memberChain.Push(methodInfo);

            MethodInfo baseMethod = methodInfo.GetBaseDefinition();
            Type curType = methodInfo.DeclaringType.BaseType;
            while (true) {
                MemberInfo[] membInfos = curType.GetMember(methodInfo.Name, MemberTypes.Method,
                    BindingFlags.IgnoreCase | BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Public);
                MethodInfo foundMatch = null;
                foreach (MemberInfo memberInfo in membInfos) {
                    MethodInfo mi = (MethodInfo)memberInfo;
                    if (mi.GetBaseDefinition() == baseMethod && mi.DeclaringType == curType) {
                        foundMatch = mi;
                        break;
                    }
                }
                if (foundMatch == null) {
                    break;
                }
                memberChain.Push(foundMatch);
                curType = curType.BaseType;
            }

            return SortActionFilters(memberChain);
        }

        protected virtual void RedirectToAction(RouteValueDictionary values) {
            VirtualPathData vpd = RouteCollection.GetVirtualPath(ControllerContext, values);
            string target = null;
            if (vpd != null) {
                target = vpd.VirtualPath;
            }
            HttpContext.Response.Redirect(target);
        }

        protected void RedirectToAction(string actionName) {
            if (String.IsNullOrEmpty(actionName)) {
                throw new ArgumentException(MvcResources.Common_NullOrEmpty, "actionName");
            }
            RouteValueDictionary valuesDictionary = new RouteValueDictionary();
            valuesDictionary.Add("action", actionName);
            RedirectToAction(valuesDictionary);
        }

        protected void RedirectToAction(string actionName, string controllerName) {
            if (String.IsNullOrEmpty(actionName)) {
                throw new ArgumentException(MvcResources.Common_NullOrEmpty, "actionName");
            }
            if (String.IsNullOrEmpty(controllerName)) {
                throw new ArgumentException(MvcResources.Common_NullOrEmpty, "controllerName");
            }
            RouteValueDictionary valuesDictionary = new RouteValueDictionary();
            valuesDictionary.Add("action", actionName);
            valuesDictionary.Add("controller", controllerName);
            RedirectToAction(valuesDictionary);
        }

        protected void RenderView(string viewName) {
            RenderView(viewName, String.Empty, ViewData);
        }

        protected void RenderView(string viewName, string masterName) {
            RenderView(viewName, masterName, ViewData);
        }

        protected void RenderView(string viewName, object viewData) {
            RenderView(viewName, String.Empty, viewData);
        }

        protected virtual void RenderView(string viewName, string masterName, object viewData) {
            ViewContext viewContext = new ViewContext(ControllerContext, viewName, masterName, viewData, TempData);
            ViewEngine.RenderView(viewContext);
        }

        private static List<ActionFilterAttribute> SortActionFilters(Stack<MemberInfo> memberChain) {
            List<ActionFilterAttribute> filters = new List<ActionFilterAttribute>();

            foreach (MemberInfo member in memberChain) {
                ActionFilterAttribute[] attrs = (ActionFilterAttribute[])member.GetCustomAttributes(typeof(ActionFilterAttribute), false /* inherit */);
                SortedList<int, ActionFilterAttribute> orderedFilters = new SortedList<int, ActionFilterAttribute>();
                foreach (ActionFilterAttribute filter in attrs) {
                    // filters are allowed to have the same order only if the order is -1.  in that case,
                    // they are processed before explicitly ordered filters but in no particular order in
                    // relation to one another.
                    if (filter.Order >= 0) {
                        if (orderedFilters.ContainsKey(filter.Order)) {
                            throw new InvalidOperationException(
                                String.Format(
                                    CultureInfo.CurrentUICulture,
                                    MvcResources.ActionFilter_DuplicateOrder,
                                    member,
                                    filter.Order));
                        }
                        orderedFilters.Add(filter.Order, filter);
                    }
                    else {
                        filters.Add(filter);
                    }
                }
                filters.AddRange(orderedFilters.Values);
            }

            return filters;
        }

        #region IController Members
        void IController.Execute(ControllerContext controllerContext) {
            Execute(controllerContext);
        }
        #endregion

        private sealed class ControllerActionFilter : ActionFilterAttribute {

            private Controller _controller;

            public ControllerActionFilter(Controller controller) {
                _controller = controller;
            }

            public override void OnActionExecuted(FilterExecutedContext filterContext) {
                _controller.OnActionExecuted(filterContext);
            }

            public override void OnActionExecuting(FilterExecutingContext filterContext) {
                _controller.OnActionExecuting(filterContext);
            }

        }

    }
}
