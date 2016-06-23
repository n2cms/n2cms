using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using N2.Web.Mvc.Html;
using N2.Engine;
using N2.Web.Targeting;

namespace N2.Web.Mvc
{
    /// <summary>
    /// A view engine that tries to retrieve resources from a theme folder before fallback to the default location.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ThemeViewEngine<T> : IViewEngine, IDecorator<IViewEngine>
        where T : VirtualPathProviderViewEngine, new()
    {
        Engine.Logger<ThemeViewEngine<T>> logger;

        Dictionary<string, T> engines = new Dictionary<string, T>();
        string themeFolderPath;
        private string[] viewExtensions;
        private string[] masterExtensions;

        /// <summary>Search for views in ~/Themes/Default/Views... as fallback to any theme preference.</summary>
        public bool FallbackToDefaultTheme { get; set; }

        /// <summary>Search for view in ~/Views/...</summary>
        public bool FallbackToRootViews { get; set; }

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

            FallbackToDefaultTheme = true;
            FallbackToRootViews = true;
        }

        #region IViewEngine Members

        public ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache)
        {
            if (controllerContext.IsChildAction == false)
            {
                controllerContext.InitTheme();
            }
            string theme = controllerContext.GetTheme();
            var engine = GetOrCreateViewEngine(controllerContext, theme);

            var ctx = controllerContext.HttpContext.GetTargetingContext();
            foreach (var detector in ctx.TargetedBy)
            {
                var result = engine.FindPartialView(controllerContext, (partialViewName ?? "Index") + "_" + detector.Name, useCache);
                if (result.View != null)
                    return result;
            }

            return engine.FindPartialView(controllerContext, partialViewName, useCache);
        }

        public ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
        {
            controllerContext.InitTheme();
            string theme = controllerContext.GetTheme();
            var engine = GetOrCreateViewEngine(controllerContext, theme);

            var ctx = controllerContext.HttpContext.GetTargetingContext();
            foreach (var detector in ctx.TargetedBy)
            {
                var result = engine.FindView(controllerContext, (viewName ?? "Index") + "_" + detector.Name, masterName, useCache);
                if (result.View != null)
                    return result;
            }

            return engine.FindView(controllerContext, viewName, masterName, useCache);
        }

        public void ReleaseView(ControllerContext controllerContext, IView view)
        {
            string theme = controllerContext.GetTheme();
            var engine = GetOrCreateViewEngine(controllerContext, theme);
            engine.ReleaseView(controllerContext, view);
        }

        private VirtualPathProviderViewEngine GetOrCreateViewEngine(ControllerContext controllerContext, string theme)
        {
            if(string.IsNullOrEmpty(theme))
                theme = "Default";

            T engine;
            if (!engines.TryGetValue(theme, out engine))
            {
                string themePath = themeFolderPath + theme + "/";
                string defaultThemePath = themeFolderPath + "Default/";
                string rootViewsPath = "~/";

				var paths = new List<string>();
				if (defaultThemePath != themePath)
					paths.Add(themePath);
				if (FallbackToRootViews)
					paths.Add(rootViewsPath);
				if (FallbackToDefaultTheme)
                    paths.Add(defaultThemePath);

                logger.InfoFormat("Creating themed view engine for theme {0} below paths {1}", theme, string.Join(", ", paths));

                engine = new T();
                engine.AreaMasterLocationFormats = GetAreaLocations(paths, masterExtensions);
                engine.AreaViewLocationFormats = GetAreaLocations(paths, viewExtensions);
                engine.AreaPartialViewLocationFormats = engine.AreaViewLocationFormats;
                engine.MasterLocationFormats = GetLocations(paths, masterExtensions);
                engine.ViewLocationFormats = GetLocations(paths, viewExtensions);
                engine.PartialViewLocationFormats = engine.ViewLocationFormats;
                engine.ViewLocationCache = new ThemeViewLocationCache();
                Utility.TrySetProperty(engine, "FileExtensions", viewExtensions);

                var temp = new Dictionary<string, T>(engines);
                temp[theme] = engine;
                engines = temp;
            }

            if (controllerContext != null)
                controllerContext.RouteData.DataTokens["ThemeViewEngine.ThemeFolderPath"] = themeFolderPath;

            return engine;
        }

        private string[] GetAreaLocations(IEnumerable<string> paths, IEnumerable<string> extensions)
        {
            return paths.SelectMany(p =>
                extensions.SelectMany(ext => new[] { 
                    p + "Areas/{2}/Views/{1}/{0}." + ext, 
                    p + "Areas/{2}/Views/Shared/{0}." + ext })).ToArray();
        }

        private string[] GetLocations(IEnumerable<string> paths, IEnumerable<string> extensions)
        {
            return paths.SelectMany(p =>
                extensions.SelectMany(ext => new[] {
                    p + "Views/{1}/{0}." + ext, 
                    p + "Views/Shared/{0}." + ext })).ToArray();
        }

        #endregion

        public IViewEngine Component
        {
            get { return GetOrCreateViewEngine(null, null); }
        }
    }
}
