using System.Web.Mvc;
using N2.Definitions;
using N2.Engine;
using System.Collections.Generic;

namespace N2.Web.Mvc.Html
{
    public static class EngineExnteions
    {
        /// <summary>Resolves the current <see cref="IEngine"/> using information stored in context or falling back to the singleton instance.</summary>
        public static IEngine ContentEngine(this HtmlHelper html)
        {
            return RouteExtensions.GetEngine(html.ViewContext.RouteData)
                ?? N2.Context.Current;
        }

        public static ItemDefinition Definition(this HtmlHelper html, ContentItem item)
        {
            return ContentEngine(html).Definitions.GetDefinition(item);
        }

        public static T ResolveService<T>(this HtmlHelper helper) where T : class
        {
            return RouteExtensions.ResolveService<T>(helper.ViewContext.RouteData);
        }

        public static IEnumerable<T> ResolveServices<T>(this HtmlHelper helper) where T : class
        {
            return RouteExtensions.ResolveServices<T>(helper.ViewContext.RouteData);
        }
    }
}
