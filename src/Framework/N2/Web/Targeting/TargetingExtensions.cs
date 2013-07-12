using N2.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Web.Targeting
{
	public static class TargetingExtensions
	{
		public static TargetingContext GetTargetingContext(this System.Web.HttpContext httpContext, IEngine engine = null)
		{
			return GetTargetingContext(httpContext.GetHttpContextBase(), engine);
		}

		public static TargetingContext GetTargetingContext(this System.Web.HttpContextBase httpContext, IEngine engine = null)
		{
			var context = httpContext.Items["N2.TargetingContext"] as TargetingContext;
			if (context == null)
			{
				if (httpContext.Request["targets"] != null && httpContext.GetEngine(engine).SecurityManager.IsEditor(httpContext.User))
				{
					var targets = httpContext.Request["targets"].Split(',');
					return new TargetingContext(httpContext) { TargetedBy = GetRadar(httpContext, engine).Detectors.Where(d => targets.Contains(d.Name)).ToList() };
				}
				else
					httpContext.Items["N2.TargetingContext"] = context = GetRadar(httpContext, engine).BuildTargetingContext(httpContext);
			}
			return context;
		}

		private static TargetingRadar GetRadar(System.Web.HttpContextBase httpContext, IEngine engine)
		{
			return httpContext.GetEngine(engine).Resolve<TargetingRadar>();
		}

		public static IEnumerable<string> GetTargetedPaths(this TargetingContext context, string templateUrl)
		{
			var url = templateUrl.ToUrl();
			var file = url.Segments.LastOrDefault();
			var dir = url.RemoveTrailingSegment(maintainExtension: false);
			
			foreach (var target in context.TargetedBy)
			{
				yield return dir.AppendSegment(target.Name, useDefaultExtension: false).AppendSegment(file);
			}

			yield return templateUrl;
		}
	}
}
