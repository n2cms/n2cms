using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using N2.Web.Mvc.Html;

namespace N2.Web.Mvc
{
	public class ThemeViewEngine<T> : IViewEngine where T : VirtualPathProviderViewEngine, new()
	{
		Dictionary<string, T> engines = new Dictionary<string, T>();

		public ThemeViewEngine()
		{
		}

		#region IViewEngine Members

		public ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache)
		{
			string theme = controllerContext.GetTheme();
			var engine = GetOrCreateViewEngine(theme);
			return engine.FindPartialView(controllerContext, partialViewName, useCache);
		}

		public ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
		{
			controllerContext.InitTheme();
			string theme = controllerContext.GetTheme();
			var engine = GetOrCreateViewEngine(theme);
			return engine.FindView(controllerContext, viewName, masterName, useCache);
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
				engine.FileExtensions = new string[] { "cshtml", "vbhtml" };

				var temp = new Dictionary<string, T>(engines);
				temp[theme] = engine;
				engines = temp;
			}

			return engine;
		}

		#endregion
	}
}