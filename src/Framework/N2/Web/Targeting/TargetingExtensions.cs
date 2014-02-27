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
                if (!TargetingRadar.Enabled)
                    return new TargetingContext(httpContext);

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
            if (string.IsNullOrEmpty(templateUrl))
                yield break;

            var extension = Url.GetExtension(templateUrl);
            if (string.IsNullOrEmpty(extension))
            {
                if (templateUrl[templateUrl.Length - 1] != '/')
                    yield break;

                foreach (var target in context.TargetedBy)
                {
                    yield return templateUrl + target.Name + "/";
                }
                yield break;
            }

            var templateUrlWithoutExtension = templateUrl.Substring(0, templateUrl.Length - extension.Length);
            foreach (var target in context.TargetedBy)
            {
                yield return templateUrlWithoutExtension + "_" + target.Name + extension;
            }
        }
    }
}
