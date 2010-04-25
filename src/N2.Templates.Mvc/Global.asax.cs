using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using N2.Engine;
using N2.Templates.Mvc.Controllers;
using N2.Web.Mvc;

namespace N2.Templates.Mvc
{
	// Note: For instructions on enabling IIS6 or IIS7 classic mode, 
	// visit http://go.microsoft.com/?LinkId=9394801

	public class MvcApplication : HttpApplication
	{
		class ViewEngine : WebFormViewEngine
		{
			public override ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
			{

				return base.FindView(controllerContext, viewName, masterName, useCache);
			}
		}

		protected void Application_Start()
		{
			var engine = MvcEngine.Create();

			engine.RegisterControllers(typeof (HomeController).Assembly);

			RegisterRoutes(RouteTable.Routes, engine);
		}

		public static void RegisterRoutes(RouteCollection routes, IEngine engine)
		{
			AreaRegistration.RegisterAllAreas(new AreaRegistrationState(engine));

			routes.MapContentRoute("Content", engine);

			routes.MapRoute(
				"Default", // Route name
				"{controller}/{action}/{id}", // URL with parameters
				new {controller = "Home", action = "Index", id = ""} // Parameter defaults
				);
		}
	}
}