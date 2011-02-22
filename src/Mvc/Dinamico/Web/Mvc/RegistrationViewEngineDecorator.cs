using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using N2.Web.Mvc.Html;

namespace N2.Web.Mvc
{
	public class RegistrationViewEngineDecorator : IViewEngine
	{
		IViewEngine inner;

		public RegistrationViewEngineDecorator(IViewEngine inner)
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

		private static void AppendRazorViewPath(ControllerContext controllerContext, ViewEngineResult result)
		{
			if (result.View == null)
				return;

			var viewContext = controllerContext as ViewContext;
			if (viewContext == null)
				return;

			var re = RegistrationExtensions.GetRegistrationExpression(viewContext.ViewData);
			if (re == null)
				return;

			var razorView = result.View as RazorView;
			if (razorView == null)
				return;

			re.TouchedPaths.Add(razorView.ViewPath);
		}
	}
}