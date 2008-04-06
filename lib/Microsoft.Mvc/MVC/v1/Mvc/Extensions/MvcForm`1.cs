namespace System.Web.Mvc {
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Web;
    using System.Web.Routing;

    /// <summary>
    /// A class representing an HTML form. Access this object using Html.Form<T>()
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MvcForm<TController> : SimpleForm where TController : Controller {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an Extension Method which allows the user to provide a strongly-typed argument via Expression")]
        public MvcForm(HtmlHelper helper, HttpContextBase context, Expression<Action<TController>> postAction, FormMethod method, RouteValueDictionary htmlAttributes) :
            base(context, LinkExtensions.BuildUrlFromExpression(helper, postAction), method, htmlAttributes) {
        }
    }
}
