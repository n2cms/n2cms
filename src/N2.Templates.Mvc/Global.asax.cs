using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Castle.Windsor;
using MvcContrib.Castle;
using N2.Engine;
using N2.Templates.Mvc.Controllers;
using N2.Web.Mvc;

namespace N2.Templates.Mvc
{
	// Note: For instructions on enabling IIS6 or IIS7 classic mode, 
	// visit http://go.microsoft.com/?LinkId=9394801

	public class MvcApplication : HttpApplication
	{
		private readonly IWindsorContainer _container = new WindsorContainer();

		public static void RegisterRoutes(RouteCollection routes, IEngine engine)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
			routes.IgnoreRoute("Edit/{*pathInfo}");

			routes.Add(new ContentRoute(engine));

			routes.MapRoute(
				"Default",                                              // Route name
				"{controller}/{action}/{id}",                           // URL with parameters
				new { controller = "Home", action = "Index", id = "" }  // Parameter defaults
			);
		}

		protected void Application_Start()
		{
			var engine = (ContentEngine) N2.Context.Initialize(true);

			RegisterRoutes(RouteTable.Routes, engine);
			RegisterComponents(_container, engine);
		}

		private static void RegisterComponents(IWindsorContainer container, ContentEngine engine)
		{
			container.Parent = engine.Container;

			container.RegisterControllers(typeof(HomeController).Assembly);

			ControllerBuilder.Current.SetControllerFactory(new WindsorControllerFactory(container));
		}
	}
}