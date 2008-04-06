namespace System.Web.Mvc {
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;

    public static class ViewExtensions {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an Extension Method which allows the user to provide a strongly-typed argument via Expression"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "This is an Extension Method which allows the user to provide a strongly-typed argument via Expression")]
        public static string RenderComponent<TController>(this HtmlHelper helper, Expression<Action<TController>> action) where TController : ComponentController, new() {

            //instance the Controller
            TController controller = new TController();
            controller.Context = helper.ViewContext;

            //figure out which action to call
            MethodCallExpression call = action.Body as MethodCallExpression;
            if (call == null) {
                throw new InvalidOperationException("Expression must be a method call");
            }
            if (call.Object != action.Parameters[0]) {
                throw new InvalidOperationException("Method call must target lambda argument");
            }
            string actionName = call.Method.Name;

            // TODO: Need to lock this down so we don't allow invocation of arbitrary methods
            //get the method from the controller
            MethodInfo mAction = controller.GetType().GetMethod(actionName);

            //pull the arguments from the passed-in lambda
            List<object> args = new List<object>();
            foreach (Expression x in call.Arguments) {
                // TODO: Verify that the arguments are all really constants instead of just assuming that they are such
                ConstantExpression c = (ConstantExpression)x;
                object val = c.Value;
                args.Add(val);
            }

            //invoke
            mAction.Invoke(controller, args.ToArray());
            string htmlResponse = controller.RenderedHtml;
            return htmlResponse;
        }
    }
}
