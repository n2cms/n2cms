using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using N2.Web.Mvc.Html;

namespace N2.Web.Mvc
{
	public class RegisteringViewEngineDecorator : IViewEngine
	{
		IViewEngine inner;

		public RegisteringViewEngineDecorator(IViewEngine inner)
		{
			this.inner = inner;
		}

		#region IViewEngine Members

		public ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache)
		{
			var result = inner.FindPartialView(controllerContext, partialViewName, useCache);
			AppendRazorViewPath(controllerContext, result);
			return result;
		}

		public ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
		{
			var result = inner.FindView(controllerContext, viewName, masterName, useCache);
			AppendRazorViewPath(controllerContext, result);
			return result;
		}

		public void ReleaseView(ControllerContext controllerContext, IView view)
		{
			inner.ReleaseView(controllerContext, view);
		}

		#endregion

		public override bool Equals(object obj)
		{
			return inner.Equals(obj);
		}

		public override int GetHashCode()
		{
			return inner.GetHashCode();
		}

		private static void AppendRazorViewPath(ControllerContext controllerContext, ViewEngineResult result)
		{
			if (result.View == null)
				return;

			var viewContext = controllerContext as ViewContext;
			if (viewContext == null)
				return;

			var re = RegistrationExtensions.GetRegistrationExpression(viewContext.HttpContext);
			if (re == null)
				return;

			var viewPath = Utility.GetProperty(result.View, "ViewPath") as string;
			if (viewPath == null)
				return;

			re.Context.TouchedPaths.Add(viewPath);
		}
	}
}