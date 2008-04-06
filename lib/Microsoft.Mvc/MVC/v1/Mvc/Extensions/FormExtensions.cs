namespace System.Web.Mvc {
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Web.Routing;

    public static class FormExtensions {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an Extension Method which allows the user to provide a strongly-typed argument via Expression")]
        public static IDisposable Form<T>(this HtmlHelper helper, Expression<Action<T>> postAction) where T : Controller {
            return Form<T>(helper, postAction, FormMethod.Post, null);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an Extension Method which allows the user to provide a strongly-typed argument via Expression")]
        public static IDisposable Form<T>(this HtmlHelper helper, Expression<Action<T>> postAction, FormMethod method) where T : Controller {
            return Form<T>(helper, postAction, method, null);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an Extension Method which allows the user to provide a strongly-typed argument via Expression")]
        public static IDisposable Form<T>(this HtmlHelper helper, Expression<Action<T>> postAction, FormMethod method, object htmlAttributes) where T : Controller {
            MvcForm<T> form = new MvcForm<T>(helper, helper.ViewContext.HttpContext, postAction, method, new RouteValueDictionary(htmlAttributes));
            form.WriteStartTag();
            return form;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an Extension Method which allows the user to provide a strongly-typed argument via Expression")]
        public static IDisposable Form<T>(this HtmlHelper helper, Expression<Action<T>> postAction, FormMethod method, IDictionary<string, object> htmlAttributes) where T : Controller {
            MvcForm<T> form = new MvcForm<T>(helper, helper.ViewContext.HttpContext, postAction, method, (htmlAttributes == null) ? new RouteValueDictionary() : new RouteValueDictionary(htmlAttributes));
            form.WriteStartTag();
            return form;
        }

        public static IDisposable Form(this HtmlHelper helper, string controllerName, string actionName) {
            return Form(helper, controllerName, actionName, FormMethod.Post, null);
        }

        public static IDisposable Form(this HtmlHelper helper, string controllerName, string actionName, FormMethod method) {
            return Form(helper, controllerName, actionName, method, null);
        }

        public static IDisposable Form(this HtmlHelper helper, string controllerName, string actionName, FormMethod method, IDictionary<string, object> htmlAttributes) {
            RouteValueDictionary values = new RouteValueDictionary();

            values.Add("controller", controllerName);
            values.Add("action", actionName);

            VirtualPathData vpd = RouteTable.Routes.GetVirtualPath(helper.ViewContext, values);
            string formAction = (vpd == null) ? null : vpd.VirtualPath;

            SimpleForm form = new SimpleForm(helper.ViewContext.HttpContext, formAction, method, (htmlAttributes == null) ? new RouteValueDictionary() : new RouteValueDictionary(htmlAttributes));
            form.WriteStartTag();
            return form;
        }
    }
}
