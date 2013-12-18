using System;
using System.Web.Mvc;
using System.Text.RegularExpressions;

namespace N2.Web.Mvc
{
    /// <summary>
    /// Automatically associates a controller external to N2 and not routed through a content 
    /// controller with an item on which details can be written and parts associated.
    /// </summary>
    /// <example>
    ///     // route registration
    ///     context.MapRoute("hello",
    ///         "Tests/{controller}/{action}/{id}",
    ///         new { action = "Index", id = UrlParameter.Optional },
    ///         new { area = new NonContentConstraint() }
    ///     );
    ///
    ///     // controller
    ///     [ExternalContent("id")]
    ///     public class StaticController : Controller
    ///     {
    ///         // This action and the view will have an external item associated to the "id" route value
    ///         public ActionResult Index(string id)
    ///         {
    ///         }
    ///     }
    /// </example>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class ExternalContentAttribute : ActionFilterAttribute
    {
        string keyRouteParameter;

        /// <summary>Creates external content defining the controller name as key for the external item.</summary>
        /// <param name="keyParameter"></param>
        public ExternalContentAttribute()
        {
        }

        /// <summary>Creates external content defining which route parameter should be used to determine they key for the external item.</summary>
        /// <param name="keyParameter"></param>
        public ExternalContentAttribute(string keyRouteParameter)
        {
            this.keyRouteParameter = keyRouteParameter;
        }

        /// <summary>Optional external content family to use, the controller name is used by default.</summary>
        public string Family { get; set; }

        /// <summary>Optional route value key where the family is retrieved.</summary>
        public string FamilyRouteParameter { get; set; }

        /// <summary>An expression that filters the key to prevent spam.</summary>
        public string KeyFilterExpression { get; set; }

        /// <summary>The type of content item to associate with this controller.</summary>
        public Type ContentType { get; set; }

        /// <summary>Applies the external content item to the route data.</summary>
        /// <param name="filterContext">The context which is modified.</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.RouteData.CurrentItem() != null)
                return;

            string family = Family ?? filterContext.RouteData.Values[FamilyRouteParameter ?? "controller"] as string;
            string key = keyRouteParameter == null ? "" : filterContext.RouteData.Values[keyRouteParameter] as string;
            if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(KeyFilterExpression))
                key = Regex.Match(key, KeyFilterExpression, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline).Value;

            string url = filterContext.RequestContext.HttpContext.Request.Url.PathAndQuery;
            filterContext.RouteData.ApplyExternalContent(family, key, url, contentType: ContentType);
        }
    }
}
