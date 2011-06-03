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
			this.themeFolderPath = themeFolderPath.Trim('~', '/');
			this.viewExtensions = fileExtensions;
			this.masterExtensions = masterExtensions;
		}

		#region IViewEngine Members

		public ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache)
		{
			string theme = controllerContext.GetTheme();
			var engine = GetOrCreateViewEngine(theme);
			var result = engine.FindPartialView(controllerContext, partialViewName, useCache);
			return result;
		}

		public ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
		{
			controllerContext.InitTheme();
			string theme = controllerContext.GetTheme();
			var engine = GetOrCreateViewEngine(theme);
			var result = engine.FindView(controllerContext, viewName, masterName, useCache);
			return result;
		}

		public void ReleaseView(ControllerContext controllerContext, IView view)
		{
			string theme = controllerContext.GetTheme();
			var engine = GetOrCreateViewEngine(theme);
			engine.ReleaseView(controllerContext, view);
		}

		private IViewEngine GetOrCreateViewEngine(string theme)
		{
			if(string.IsNullOrEmpty(theme))
				theme = "Default";

			T engine;
			if (!engines.TryGetValue(theme, out engine))
			{
				string defaultThemePath = "~/" + themeFolderPath + "/Default/";
				string themePath = "~/" + themeFolderPath + "/" + theme + "/";
			
				engine = new T();
				engine.AreaViewLocationFormats = GetAreaLocations(defaultThemePath, themePath, viewExtensions);
					//new string[] { themePath + "/Areas/{2}/Views/{1}/{0}.cshtml", themePath + "/Areas/{2}/Views/{1}/{0}.vbhtml", themePath + "/Areas/{2}/Views/Shared/{0}.cshtml", themePath + "/Areas/{2}/Views/Shared/{0}.vbhtml" };
				engine.AreaMasterLocationFormats = GetAreaLocations(defaultThemePath, themePath, masterExtensions);
					// = new string[] { themePath + "/Areas/{2}/Views/{1}/{0}.cshtml", themePath + "/Areas/{2}/Views/{1}/{0}.vbhtml", themePath + "/Areas/{2}/Views/Shared/{0}.cshtml", themePath + "/Areas/{2}/Views/Shared/{0}.vbhtml" };
				engine.AreaPartialViewLocationFormats = engine.AreaViewLocationFormats;
					// = new string[] { themePath + "/Areas/{2}/Views/{1}/{0}.cshtml", themePath + "/Areas/{2}/Views/{1}/{0}.vbhtml", themePath + "/Areas/{2}/Views/Shared/{0}.cshtml", themePath + "/Areas/{2}/Views/Shared/{0}.vbhtml" };
				engine.ViewLocationFormats = GetLocations(defaultThemePath, themePath, viewExtensions);
					//new string[] { themePath + "/Views/{1}/{0}.cshtml", themePath + "/Views/{1}/{0}.vbhtml", themePath + "/Views/Shared/{0}.cshtml", themePath + "/Views/Shared/{0}.vbhtml" };
				engine.MasterLocationFormats = GetLocations(defaultThemePath, themePath, masterExtensions);
				engine.PartialViewLocationFormats = engine.ViewLocationFormats;
					//new string[] { themePath + "/Views/{1}/{0}.cshtml", themePath + "/Views/{1}/{0}.vbhtml", themePath + "/Views/Shared/{0}.cshtml", themePath + "/Views/Shared/{0}.vbhtml" };
				Utility.TrySetProperty(engine, "FileExtensions", viewExtensions);

				var temp = new Dictionary<string, T>(engines);
				temp[theme] = engine;
				engines = temp;
			}

			return engine;
		}

		private string[] GetAreaLocations(string defaultThemePath, string themePath, string[] extensions)
		{
			return extensions.SelectMany(ext => new[] { 
					themePath + "Areas/{2}/Views/{1}/{0}." + ext, 
					themePath + "Areas/{2}/Views/Shared/{0}." + ext, 
					defaultThemePath + "Areas/{2}/Views/{1}/{0}." + ext, 
					defaultThemePath + "Areas/{2}/Views/Shared/{0}." + ext }).ToArray();
		}

		private string[] GetLocations(string defaultThemePath, string themePath, string[] extensions)
		{
			return extensions.SelectMany(ext => new[] { 
					themePath + "Views/{1}/{0}." + ext, 
					themePath + "Views/Shared/{0}." + ext, 
					defaultThemePath + "Views/{1}/{0}." + ext, 
					defaultThemePath + "Views/Shared/{0}." + ext }).ToArray();
		}

		#endregion
	}
}