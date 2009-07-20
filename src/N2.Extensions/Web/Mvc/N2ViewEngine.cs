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
			viewName = TransformViewName(controllerContext, viewName);

			return _innerViewEngine.FindView(controllerContext, viewName, masterName, useCache);
		}

		private string TransformViewName(ControllerContext controllerContext, string viewName)
		{
			var item = (ContentItem)controllerContext.RouteData.Values[ContentRoute.ContentItemKey];

			if (item != null)
				viewName = GetTemplateUrl(item, viewName);
			return viewName;
		}

		public void ReleaseView(ControllerContext controllerContext, IView view)
		{
			_innerViewEngine.ReleaseView(controllerContext, view);
		}
	}
}