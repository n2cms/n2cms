using System;
using System.Linq;
using System.Web.Mvc;

namespace N2.Web.Mvc
{
	public class N2ViewEngine : IViewEngine
	{
		private readonly IViewEngine _innerViewEngine;

		public N2ViewEngine()
			: this(new WebFormViewEngine())
		{
		}

		public N2ViewEngine(IViewEngine inner)
		{
			_innerViewEngine = inner;
		}

		protected virtual string GetTemplateUrl(ContentItem item, string viewName)
		{
			var pathData = PathDictionary
				.GetFinders(item.GetType())
				.Select(finder => finder.GetPath(item, viewName))
				.FirstOrDefault(path => path != null && !path.IsEmpty());

			var templateUrl = item.TemplateUrl;

			if (pathData != null)
				templateUrl = pathData.TemplateUrl;

			return templateUrl;
		}

		public ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache)
		{
			return _innerViewEngine.FindPartialView(controllerContext, partialViewName, useCache);
		}

		public ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
		{
			var item = (ContentItem) controllerContext.RouteData.Values[ContentRoute.ContentItemKey];

			if (item == null || IsFullPathToView(viewName))
				return _innerViewEngine.FindView(controllerContext, viewName, masterName, useCache);

			var oldControllerName = controllerContext.RouteData.Values["controller"];

			var templateUrl = GetTemplateUrl(item, viewName);

			if (IsFullPathToView(templateUrl))
				return _innerViewEngine.FindView(controllerContext, templateUrl, masterName, useCache);

			try
			{
				controllerContext.RouteData.Values["controller"] = templateUrl;

				return _innerViewEngine.FindView(controllerContext, viewName, masterName, useCache);
			}
			finally
			{
				controllerContext.RouteData.Values["controller"] = oldControllerName;
			}
		}

		private bool IsFullPathToView(string viewName)
		{
			return viewName[0] == '~' || viewName[0] == '/';
		}

		public void ReleaseView(ControllerContext controllerContext, IView view)
		{
			_innerViewEngine.ReleaseView(controllerContext, view);
		}
	}
}