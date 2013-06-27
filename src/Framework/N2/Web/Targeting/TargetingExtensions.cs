using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Web.Targeting
{
	public static class TargetingExtensions
	{
		public static TargetingContext GetTargetingContext(this System.Web.HttpContext httpContext)
		{
			return httpContext.GetHttpContextBase().GetTargetingContext();
		}

		public static TargetingContext GetTargetingContext(this System.Web.HttpContextBase httpContext)
		{
			var context = httpContext.Items["N2.TargetingContext"] as TargetingContext;
			if (context == null)
				httpContext.Items["N2.TargetingContext"] = context = httpContext.GetEngine().Resolve<TargetingRadar>().BuildTargetingContext(httpContext);
			return context;
		}
	}
}
