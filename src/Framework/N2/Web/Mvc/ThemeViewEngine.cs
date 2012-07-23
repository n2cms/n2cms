using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using N2.Web.Mvc.Html;

namespace N2.Web.Mvc
{
	/// <summary>
	/// A view engine that tries to retrieve resources from a theme folder before fallback to the default location.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class ThemeViewEngine<T> : IViewEngine where T : VirtualPathProviderViewEngine, new()
	{
		Engine.Logger<ThemeViewEngine<T>> logger;

		Dictionary<string, T> engines = new Dictionary<string, T>();
		string themeFolderPath;
		private string[] viewExtensions;
		private string[] masterExtensions;

		public ThemeViewEngine()
			: this("~/Themes/", new string[] { "cshtml", "vbhtml" }, new string[] { "cshtml", "vbhtml" })
		{
		}

		/// <summary>
		/// File extensions are only assigned when using ASP.NET MVC 3.0 or greater.
		/// </summary>
		/// <param name="fileExtensions"></param>
		public ThemeViewEngine(string themeFolderPath, string[] fileExtensions, string[] masterExtensions)
		{
			this.themeFolderPath = themeFolderPath;
			this.viewExtensions = fileExtensions;
			this.masterExtensions = masterExtensions;
		}

		#region IViewEngine Members

		public ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache)
		{
			string theme = controllerContext.GetTheme();
			var engine = GetOrCreateViewEngine(controllerContext, theme);
			var result = engine.FindPartialView(controllerContext, partialViewName, useCache);
			return result;
		}

		public ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
		{
			controllerContext.InitTheme();
			string theme = controllerContext.GetTheme();
			var engine = GetOrCreateViewEngine(controllerContext, theme);
			var result = engine.FindView(controllerContext, viewName, masterName, useCache);
			return result;
		}

		public void ReleaseView(ControllerContext controllerContext, IView view)
		{
			string theme = controllerContext.GetTheme();
			var engine = GetOrCreateViewEngine(controllerContext, theme);
			engine.ReleaseView(controllerContext, view);
		}

		private IViewEngine GetOrCreateViewEngine(ControllerContext controllerContext, string theme)
		{
			if(string.IsNullOrEmpty(theme))
				theme = "Default";

			T engine;
			if (!engines.TryGetValue(theme, out engine))
			{
				string fallbackPath = themeFolderPath + "Default/";
				string themePath = themeFolderPath + theme + "/";

				logger.InfoFormat("Creating themed view engine for theme {0} below path {1}", theme, themePath);

				engine = new T();
				engine.AreaMasterLocationFormats = GetAreaLocations(themePath, fallbackPath, masterExtensions);
				engine.AreaViewLocationFormats = GetAreaLocations(themePath, fallbackPath, viewExtensions);
				engine.AreaPartialViewLocationFormats = engine.AreaViewLocationFormats;
				engine.MasterLocationFormats = GetLocations(themePath, fallbackPath, masterExtensions);
				engine.ViewLocationFormats = GetLocations(themePath, fallbackPath, viewExtensions);
				engine.PartialViewLocationFormats = engine.ViewLocationFormats;
				engine.ViewLocationCache = new ThemeViewLocationCache();
				Utility.TrySetProperty(engine, "FileExtensions", viewExtensions);

				var temp = new Dictionary<string, T>(engines);
				temp[theme] = engine;
				engines = temp;
			}

			controllerContext.RouteData.DataTokens["ThemeViewEngine.ThemeFolderPath"] = themeFolderPath;

			return engine;
		}

		private string[] GetAreaLocations(string themePath, string fallbackPath, string[] extensions)
		{
			return extensions.SelectMany(ext => new[] { 
					themePath + "Areas/{2}/Views/{1}/{0}." + ext, 
					themePath + "Areas/{2}/Views/Shared/{0}." + ext, 
					fallbackPath + "Areas/{2}/Views/{1}/{0}." + ext, 
					fallbackPath + "Areas/{2}/Views/Shared/{0}." + ext }).ToArray();
		}

		private string[] GetLocations(string themePath, string fallbackPath, string[] extensions)
		{
			return extensions.SelectMany(ext => new[] { 
					themePath + "Views/{1}/{0}." + ext, 
					themePath + "Views/Shared/{0}." + ext, 
					fallbackPath + "Views/{1}/{0}." + ext, 
					fallbackPath + "Views/Shared/{0}." + ext }).ToArray();
		}

		#endregion
	}
}