using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.UI;
using System.Web;
using N2.Web.UI;
using System.Security.Principal;
using System.Web.Routing;

namespace N2.Web.Mvc
{
	/// <summary>
	/// Enables output cache as configured in the n2/host/outputCache configuration 
	/// esction with cache dependency. This functionality is enabled by default when
	/// inheriting from <see cref="ContentController"/>.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
	public class ContentOutputCacheAttribute : ActionFilterAttribute
	{
		public override void OnResultExecuting(ResultExecutingContext filterContext)
		{
			if (filterContext == null)
				throw new ArgumentNullException("filterContext");

			if (filterContext.IsChildAction)
				return;

			ProcessOutputCache(filterContext.RequestContext);
		}

		private static void ProcessOutputCache(RequestContext requestContext)
		{
			if (HttpContext.Current == null)
				return;

			var user = requestContext.HttpContext.User;
			if (user == null || user.Identity.IsAuthenticated)
				return;

			ICacheManager cacheManager = requestContext.RouteData.ResolveService<ICacheManager>();
			if (!cacheManager.Enabled)
				return;

			var page = new OutputCachedPage(cacheManager.GetOutputCacheParameters());
			page.ProcessRequest(HttpContext.Current);
			cacheManager.AddCacheInvalidation(HttpContext.Current.Response);
		}

		private sealed class OutputCachedPage : Page
		{
			private OutputCacheParameters _cacheSettings;

			public OutputCachedPage(OutputCacheParameters cacheSettings)
			{
				this.ID = Guid.NewGuid().ToString();
				this._cacheSettings = cacheSettings;
			}

			protected override void FrameworkInitialize()
			{
				base.FrameworkInitialize();
				this.InitOutputCache(this._cacheSettings);
			}
		}
	}
}
