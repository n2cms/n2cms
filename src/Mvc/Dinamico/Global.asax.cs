using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using N2.Web.Mvc;
using N2.Engine;
using System.Reflection;
using Dinamico.Definitions.Dynamic;

namespace Dinamico
{
	// Note: For instructions on enabling IIS6 or IIS7 classic mode, 
	// visit http://go.microsoft.com/?LinkId=9394801

	public class MvcApplication : System.Web.HttpApplication
	{
		protected void Application_Start()
		{
			//log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo(Server.MapPath("~/N2/Installation/log4net.config")));

			var engine = N2.Context.Current;

			AreaRegistration.RegisterAllAreas();

			RegisterControllerFactory(ControllerBuilder.Current, engine);
			RegisterGlobalFilters(GlobalFilters.Filters);
			RegisterRoutes(RouteTable.Routes, engine);
			RegisterViewEngines(ViewEngines.Engines);

			engine.Resolve<RazorTemplateRegistrator>()
				.Add<Controllers.StartPageController>()
				.Add<Controllers.ContentPagesController>()
				.Add<Controllers.ContentPartsController>()
				.Add<Controllers.ListingPagesController>();
		}

		public static void RegisterControllerFactory(ControllerBuilder controllerBuilder, IEngine engine)
		{
			engine.RegisterControllers(Assembly.GetExecutingAssembly());

			var controllerFactory = engine.Resolve<ControllerFactoryConfigurator>()
				.NotFound<Controllers.StartPageController>(sc => sc.NotFound())
				.ControllerFactory;

			controllerBuilder.SetControllerFactory(controllerFactory);
		}

		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new HandleErrorAttribute());
		}

		public static void RegisterRoutes(RouteCollection routes, IEngine engine)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.MapContentRoute("Content", engine);

			routes.MapRoute(
				"Default", // Route name
				"{controller}/{action}/{*id}", // URL with parameters
				new { action = "Index", id = UrlParameter.Optional } // Parameter defaults
			);
		}

		public static void RegisterViewEngines(ViewEngineCollection viewEngines)
		{
			viewEngines.RegisterThemeViewEngine<RazorViewEngine>();
			viewEngines.WrapForTemplateRegistration();
		}
	}
}