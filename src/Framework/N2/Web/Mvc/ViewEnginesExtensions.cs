using N2.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace N2.Web.Mvc
{
    public static class ViewEnginesExtensions
    {
        /// <summary>Register the <see cref="ThemeViewEngine"/> which redirects view renderings to a themed counterpart if available.</summary>
        /// <typeparam name="T">The type of view engine to use as base.</typeparam>
        /// <param name="viewEngines">Placeholder.</param>
        /// <returns>The theme view engine that was inserted.</returns>
        public static ThemeViewEngine<T> RegisterThemeViewEngine<T>(this ViewEngineCollection viewEngines, string themeFolderPath, string[] fileExtensions, string[] masterExtensions) 
            where T : VirtualPathProviderViewEngine, new()
        {
            Url.SetToken(Url.ThemesUrlToken, themeFolderPath);
            
            var tve = new ThemeViewEngine<T>(themeFolderPath, fileExtensions, masterExtensions);
            viewEngines.Insert(0, tve);
            return tve;
        }

        /// <summary>Register the <see cref="ThemeViewEngine"/> which redirects view renderings to a themed counterpart if available.</summary>
        /// <typeparam name="T">The type of view engine to use as base.</typeparam>
        /// <param name="viewEngines">Placeholder.</param>
        /// <returns>The theme view engine that was inserted.</returns>
        public static ThemeViewEngine<T> RegisterThemeViewEngine<T>(this ViewEngineCollection viewEngines, string themeFolderPath = "~/Themes/") 
            where T : VirtualPathProviderViewEngine, new()
        {
            Url.SetToken(Url.ThemesUrlToken, themeFolderPath);

            var tve = typeof(T) == typeof(WebFormViewEngine)
                ? new ThemeViewEngine<T>(themeFolderPath, new[] { "aspx", "ascx" }, new[] { "master" })
                : new ThemeViewEngine<T>(themeFolderPath, new[] { "cshtml" }, new[] { "cshtml" });
            viewEngines.Insert(0, tve);
            return tve;
        }

        /// <summary>Wraps all currently registered view engines enabling the template first developerment paradigm.</summary>
        /// <param name="viewEngines">Placeholder.</param>
        /// <remarks>To enable template first you must also explictly add controllers using the <see cref="N2.Definitions.Runtime.ViewTemplateRegistrator"/>.</remarks>
        public static void DecorateViewTemplateRegistration(this ViewEngineCollection viewEngines)
        {
            for (int i = 0; i < viewEngines.Count; i++)
            {
                viewEngines[i] = new RegisteringViewEngineDecorator(viewEngines[i]);
            }
        }

        /// <summary>Adds a view engine that resolves tokens from N2's management path.</summary>
        /// <param name="viewEngines">The view engines to append.</param>
        public static void RegisterTokenViewEngine(this ViewEngineCollection viewEngines)
        {
            viewEngines.Add(new TokenViewEngine());
        }

        class TokenViewEngine : IViewEngine, IDecorator<IViewEngine>
        {
            WebFormViewEngine inner;
            string[] noLocations = new string[0];

            public TokenViewEngine()
            {
                var tokenLocations = new[] { Url.ResolveTokens("{ManagementUrl}/Tokens/{0}.ascx") };
                inner = new WebFormViewEngine
                {
                    AreaMasterLocationFormats = noLocations,
                    AreaPartialViewLocationFormats = noLocations,
                    AreaViewLocationFormats = noLocations,
                    MasterLocationFormats = noLocations,
                    PartialViewLocationFormats = tokenLocations,
                    ViewLocationFormats = tokenLocations,
                    ViewLocationCache = new DefaultViewLocationCache()
                };
            }

            public ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache)
            {
                return inner.FindPartialView(controllerContext, partialViewName, useCache);
            }

            public ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
            {
                return new ViewEngineResult(noLocations);
            }

            public void ReleaseView(ControllerContext controllerContext, IView view)
            {
                inner.ReleaseView(controllerContext, view);
            }

            public IViewEngine Component
            {
                get { return inner; }
            }
        }

    }
}
