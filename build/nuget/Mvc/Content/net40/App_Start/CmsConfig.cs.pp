using System.Web.Mvc;
using System.Web.Routing;
using N2.Definitions.Runtime;
using N2.Engine;
using N2.Plugin;
using N2.Web.Mvc;

namespace $rootnamespace$.App_Start
{
	/// <summary>
	/// This configures ASP.NET MVC to use N2 for routing and optionally other features (see commented code).
	/// </summary>
	[AutoInitialize]
	public class CmsConfig : IPluginInitializer
	{
		public void Initialize(N2.Engine.IEngine engine)
		{
			// When using Dinamico check out /Dinamico/GlobalMvcStarter.cs which 
			// configures these settings specifically for Dinamico
			RegisterControllerFactory(ControllerBuilder.Current, engine);
			RegisterRoutes(RouteTable.Routes, engine);
			RegisterViewEngines(ViewEngines.Engines);
			RegisterViewTemplates(engine);
		}

		public static void RegisterControllerFactory(ControllerBuilder controllerBuilder, IEngine engine)
		{
			//// Registers controllers in the solution for dependency injection using the IoC container provided by N2
			//engine.RegisterAllControllers();
			//controllerBuilder.SetControllerFactory(engine.GetControllerFactory());
		}

		public static void RegisterRoutes(RouteCollection routes, IEngine engine)
		{
			// The content route routes firndly urls based onthe content structure
			routes.MapContentRoute("Content", engine);
		}

		public static void RegisterViewEngines(ViewEngineCollection viewEngines)
		{
			// Theeming works in combination with IThemeable on the start page to lookup 
			// views and resources below /themes/name/ before falling back to root
			
			// viewEngines.RegisterThemeViewEngine<RazorViewEngine>("~/Themes/");
		}

		public static void RegisterViewTemplates(IEngine engine)
		{
			// View templates allows defining editors in the .cshtml file rather than using attributes on the model

			//engine.RegisterViewTemplates<Controllers.ContentPagesController>()
			//	.Add<Controllers.ContentPartsController>();
			//viewEngines.DecorateViewTemplateRegistration();
		}
	}
}