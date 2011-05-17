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
		private string[] fileExtensions;

		public ThemeViewEngine()
			: this(new string[] { "cshtml", "vbhtml" })
		{
		}

		/// <summary>
		/// File extensions are only assigned when using ASP.NET MVC 3.0 or greater.
		/// </summary>
		/// <param name="fileExtensions"></param>
		public ThemeViewEngine(params string[] fileExtensions)
		{
			this.fileExtensions = fileExtensions;
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
				string themePath = "~/Themes/" + theme;
			
				engine = new T();
				engine.AreaViewLocationFormats = new string[] { themePath + "/Areas/{2}/Views/{1}/{0}.cshtml", themePath + "/Areas/{2}/Views/{1}/{0}.vbhtml", themePath + "/Areas/{2}/Views/Shared/{0}.cshtml", themePath + "/Areas/{2}/Views/Shared/{0}.vbhtml" };
				engine.AreaMasterLocationFormats = new string[] { themePath + "/Areas/{2}/Views/{1}/{0}.cshtml", themePath + "/Areas/{2}/Views/{1}/{0}.vbhtml", themePath + "/Areas/{2}/Views/Shared/{0}.cshtml", themePath + "/Areas/{2}/Views/Shared/{0}.vbhtml" };
				engine.AreaPartialViewLocationFormats = new string[] { themePath + "/Areas/{2}/Views/{1}/{0}.cshtml", themePath + "/Areas/{2}/Views/{1}/{0}.vbhtml", themePath + "/Areas/{2}/Views/Shared/{0}.cshtml", themePath + "/Areas/{2}/Views/Shared/{0}.vbhtml" };
				engine.ViewLocationFormats = new string[] { themePath + "/Views/{1}/{0}.cshtml", themePath + "/Views/{1}/{0}.vbhtml", themePath + "/Views/Shared/{0}.cshtml", themePath + "/Views/Shared/{0}.vbhtml" };
				engine.MasterLocationFormats = new string[] { themePath + "/Views/{1}/{0}.cshtml", themePath + "/Views/{1}/{0}.vbhtml", themePath + "/Views/Shared/{0}.cshtml", themePath + "/Views/Shared/{0}.vbhtml" };
				engine.PartialViewLocationFormats = new string[] { themePath + "/Views/{1}/{0}.cshtml", themePath + "/Views/{1}/{0}.vbhtml", themePath + "/Views/Shared/{0}.cshtml", themePath + "/Views/Shared/{0}.vbhtml" };
				Utility.TrySetProperty(engine, "FileExtensions", fileExtensions);

				var temp = new Dictionary<string, T>(engines);
				temp[theme] = engine;
				engines = temp;
			}

			return engine;
		}

		#endregion
	}
}