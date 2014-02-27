using System.Web;
using System.Web.Mvc;
using N2.Definitions;
using N2.Engine;
using System.Linq;
using System;

namespace N2.Web.Mvc
{
    public static class MvcExtensions
    {

        // theming

        private const string ThemeKey = "theme";
        public static string GetTheme(this ControllerContext context)
        {
            return context.HttpContext.GetTheme();
        }

        internal static string GetTheme(this HttpContextBase context)
        {
            return context.Request[ThemeKey] // preview theme via query string
                ?? context.Items[ThemeKey] as string // previously initialized theme
                ?? "Default"; // fallback
        }

        public static bool IsThemeInitialized(this ControllerContext context)
        {
            return context.HttpContext.Items[ThemeKey] != null;
        }

        public static void SetTheme(this ControllerContext context, string theme)
        {
            context.HttpContext.Items[ThemeKey] = theme;
        }

        public static void InitTheme(this ControllerContext context)
        {
            if (context.IsThemeInitialized())
                return;

            var page = context.RequestContext.CurrentPage<ContentItem>()
                ?? RouteExtensions.ResolveService<IUrlParser>(context.RouteData).FindPath(context.HttpContext.Request["returnUrl"]).StopItem
                ?? RouteExtensions.ResolveService<IUrlParser>(context.RouteData).FindPath(context.HttpContext.Request.AppRelativeCurrentExecutionFilePath).StopItem
                ?? context.RequestContext.StartPage();

            InitTheme(context, page);
        }

        private static void InitTheme(ControllerContext context, ContentItem page)
        {
            var start = Find.ClosestOf<IThemeable>(page);
            if (start == null)
                return;

            var themeSource = start as IThemeable;
            if (string.IsNullOrEmpty(themeSource.Theme))
                InitTheme(context, start.Parent);
            else
                context.SetTheme(themeSource.Theme);
        }

        /// <summary>
        /// Registers all controllers in assemblies normally considered for content definition with the Engine, so that they may be resolved correctly.
        /// </summary>
        /// <param name="engine"></param>
        public static void RegisterAllControllers(this IEngine engine)
        {
            foreach (Type type in engine.Resolve<ITypeFinder>().Find(typeof(IController)).Where(t => !t.IsAbstract).Where(t => !t.IsInterface))
                engine.Container.AddComponentLifeStyle(type.FullName.ToLower(), type, ComponentLifeStyle.Transient);
        }

		/// <summary>
		/// Gets a controller factory which uses N2 for service location.
		/// </summary>
		/// <param name="engine"></param>
		/// <returns></returns>
        public static IControllerFactory GetControllerFactory(this IEngine engine)
		{
			return engine.Resolve<ControllerFactoryConfigurator>().ControllerFactory;
		}

    }
}
