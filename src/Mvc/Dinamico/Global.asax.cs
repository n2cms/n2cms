using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using N2.Web.Mvc;
using N2.Engine;
using System.Reflection;

namespace Dinamico
{
	// Note: For instructions on enabling IIS6 or IIS7 classic mode, 
	// visit http://go.microsoft.com/?LinkId=9394801

	public class MvcApplication : System.Web.HttpApplication
	{
		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new HandleErrorAttribute());
		}

		public static void RegisterControllerFactory(ControllerBuilder controllerBuilder, IEngine engine)
		{
			engine.RegisterControllers(Assembly.GetExecutingAssembly());

			var controllerFactory = engine.Resolve<ControllerFactoryConfigurator>()
				.NotFound<Controllers.StartPageController>(sc => sc.NotFound())
				.ControllerFactory;

			controllerBuilder.SetControllerFactory(controllerFactory);
		}

		public static void RegisterRoutes(RouteCollection routes, IEngine engine)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.MapContentRoute("Content", engine);

			routes.MapRoute(
				"Default", // Route name
				"{controller}/{action}/{*id}", // URL with parameters
				new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
			);

		}

		protected void Application_Start()
		{
			RegisterControllerFactory(ControllerBuilder.Current, N2.Context.Current);

			AreaRegistration.RegisterAllAreas();

			RegisterGlobalFilters(GlobalFilters.Filters);
			RegisterRoutes(RouteTable.Routes, N2.Context.Current);
		}
	}
}