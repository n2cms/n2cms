using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace N2.Web.Mvc
{
	public class N2ViewEngine : IViewEngine
	{
		private const string DefaultViewName = "index";
		private readonly IEnumerable<IViewEngine> _innerViewEngines;

		public N2ViewEngine(IEnumerable<IViewEngine> innerViewEngines)
		{
			_innerViewEngines = innerViewEngines;
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
			var item = ExtractItemFromContext(controllerContext);

			if (item == null || IsFullPathToView(partialViewName))
				return FindPartialViewInternal(controllerContext, partialViewName, useCache);

			var templateUrl = partialViewName;

			if (String.Equals(templateUrl, DefaultViewName, StringComparison.InvariantCultureIgnoreCase))
				templateUrl = GetTemplateUrl(item, partialViewName);

			return FindPartialViewInternal(controllerContext, templateUrl, useCache);
		}

		private static ContentItem ExtractItemFromContext(ControllerContext controllerContext)
		{
			var item = (ContentItem)controllerContext.RouteData.Values[ContentRoute.ContentItemKey];

			var viewContext = controllerContext as ViewContext;
			if(viewContext != null)
			{
				item = ItemExtractor.ExtractFromModel(viewContext.ViewData.Model) ?? item;

				controllerContext.RouteData.Values[ContentRoute.ContentItemKey] = item;
			}
			return item;
		}

		public ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
		{
			var item = ExtractItemFromContext(controllerContext);

			if (item == null || IsFullPathToView(viewName))
				return FindViewInternal(controllerContext, viewName, masterName, useCache);

			var oldControllerName = controllerContext.RouteData.Values["controller"];

			var templateUrl = GetTemplateUrl(item, viewName);

			if (IsFullPathToView(templateUrl))
				return FindViewInternal(controllerContext, templateUrl, masterName, useCache);

			try
			{
				controllerContext.RouteData.Values["controller"] = templateUrl;

				return FindViewInternal(controllerContext, viewName, masterName, useCache);
			}
			finally
			{
				controllerContext.RouteData.Values["controller"] = oldControllerName;
			}
		}

		private ViewEngineResult FindPartialViewInternal(ControllerContext controllerContext, string partialViewName, bool useCache)
		{
			var searchedLocations = new List<string>();
			foreach(var engine in _innerViewEngines.Where(e => e != this))
			{
				var result = engine.FindPartialView(controllerContext, partialViewName, useCache);

				searchedLocations.AddRange(result.SearchedLocations ?? new string[0]);
				if(result.View != null)
					return result;
			}
			return new ViewEngineResult(searchedLocations);
		}

		private ViewEngineResult FindViewInternal(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
		{
			var searchedLocations = new List<string>();
			foreach (var engine in _innerViewEngines.Where(e => e != this))
			{
				var result = engine.FindView(controllerContext, viewName, masterName, useCache);

				searchedLocations.AddRange(result.SearchedLocations ?? new string[0]);
				if (result.View != null)
					return result;
			}
			return new ViewEngineResult(searchedLocations);
		}

		private static bool IsFullPathToView(string viewName)
		{
			return viewName[0] == '~' || viewName[0] == '/';
		}

		public void ReleaseView(ControllerContext controllerContext, IView view)
		{
			var disposable = view as IDisposable;

			if(disposable != null)
				disposable.Dispose();
		}
	}
}