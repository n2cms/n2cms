using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using N2.Web.Mvc;
using N2.Web.Mvc.Html;
using N2;
using N2.Definitions;

namespace N2.Web.Mvc
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
	public class HandleThemeAttribute : FilterAttribute, IResultFilter
	{
		public const string ThemeKey = "theme";
		#region IResultFilter Members

		public void OnResultExecuted(ResultExecutedContext filterContext)
		{
		}

		public void OnResultExecuting(ResultExecutingContext filterContext)
		{
			var page = filterContext.RequestContext.CurrentPage<ContentItem>();
			if (page == null)
				return;

			var start = Find.Closest<IThemeable>(page);
			if (start == null)
				return;

			filterContext.RouteData.DataTokens[ThemeKey] = start.Theme;
		}

		#endregion
	}
}