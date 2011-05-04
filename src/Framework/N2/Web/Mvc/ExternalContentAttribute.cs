using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace N2.Web.Mvc
{
	/// <summary>
	/// Automatically associates a controller external to N2 and not routed through a content 
	/// controller with an item on which details can be written and parts associated.
	/// </summary>
	/// <example>
	///		// route registration
	///		context.MapRoute("hello",
	///			"Tests/{controller}/{action}/{id}",
	///			new { action = "Index", id = UrlParameter.Optional },
	///			new { area = new NonContentConstraint() }
	///		);
	///
	///		// controller
	///		[ExternalContent("id")]
	///		public class StaticController : Controller
	///		{
	///			// This action and the view will have an external item associated to the "id" route value
	///			public ActionResult Index(string id)
	///			{
	///			}
	///		}
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

		/// <summary>Applies the external content item to the route data.</summary>
		/// <param name="filterContext">The context which is modified.</param>
		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			if (filterContext.RouteData.CurrentItem() != null)
				return;

			string family = Family ?? filterContext.RouteData.Values["controller"] as string;
			string key = keyRouteParameter == null ? "" : filterContext.RouteData.Values[keyRouteParameter] as string;
			string url = filterContext.RequestContext.HttpContext.Request.Url.PathAndQuery;
			filterContext.RouteData.ApplyExternalContent(family, key, url);
		}
	}
}
