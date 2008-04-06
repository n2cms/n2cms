namespace System.Web.Mvc {
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Web.Routing;

    public static class LinkBuilder {
        /// <summary>
        /// Builds a URL based on the Expression passed in
        /// </summary>
        /// <typeparam name="T">Controller Type Only</typeparam>
        /// <param name="context">The current ViewContext</param>
        /// <param name="action">The action to invoke</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings", Justification = "The return value is not a regular URL since it may contain ~/ ASP.NET-specific characters"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an Extension Method which allows the user to provide a strongly-typed argument via Expression"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Need to be sure the passed-in argument is of type Controller::Action")]
        public static string BuildUrlFromExpression<T>(ViewContext context, Expression<Action<T>> action) where T : Controller {
            MethodCallExpression call = action.Body as MethodCallExpression;
            if (call == null) {
                throw new InvalidOperationException("Expression must be a method call");
            }
            if (call.Object != action.Parameters[0]) {
                throw new InvalidOperationException("Method call must target lambda argument");
            }

            string actionName = call.Method.Name;
            // TODO: Use better logic to chop off the controller suffix
            string controllerName = typeof(T).Name;
            if (controllerName.EndsWith("Controller", StringComparison.OrdinalIgnoreCase)) {
                controllerName = controllerName.Remove(controllerName.Length - 10, 10);
            }

            RouteValueDictionary values = BuildParameterValuesFromExpression(call);

            values = values ?? new RouteValueDictionary();
            values.Add("controller", controllerName);
            values.Add("action", actionName);

            VirtualPathData vpd = RouteTable.Routes.GetVirtualPath(context, values);
            return (vpd == null) ? null : vpd.VirtualPath;
        }

        /// <summary>
        /// Creates a querystring as a Dictionary based on the passed-in Lambda
        /// </summary>
        /// <param name="call">The Lambda of the Controller method</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Allowing Lambda compilation to fail if it doesn't compile at run time - design-time compilation will not allow for runtime Exception")]
        public static RouteValueDictionary BuildParameterValuesFromExpression(MethodCallExpression call) {
            RouteValueDictionary result = new RouteValueDictionary();

            ParameterInfo[] parameters = call.Method.GetParameters();

            if (parameters.Length > 0) {
                for (int i = 0; i < parameters.Length; i++) {
                    Expression arg = call.Arguments[i];
                    object value;
                    ConstantExpression ce = arg as ConstantExpression;
                    if (ce != null) {
                        // If argument is a constant expression, just get the value
                        value = ce.Value;
                    }
                    else {
                        // Otherwise, convert the argument subexpression to type object,
                        // make a lambda out of it, compile it, and invoke it to get the value
                        var lambda = Expression.Lambda<Func<object>>(Expression.Convert(arg, typeof(object)));
                        try {
                            value = lambda.Compile()();
                        }
                        catch {
                            // ?????
                            value = "";
                        }
                    }
                    // Code should be added here to appropriately escape the value string
                    result.Add(parameters[i].Name, value);
                }
            }
            return result;
        }
    }
}
