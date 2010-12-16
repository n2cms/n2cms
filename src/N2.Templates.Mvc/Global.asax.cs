using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using N2.Engine;
using N2.Templates.Mvc.Controllers;
using N2.Web.Mvc;
using N2.Templates.Mvc.Web;

namespace N2.Templates.Mvc
{
	// Note: For instructions on enabling IIS6 or IIS7 classic mode, 
	// visit http://go.microsoft.com/?LinkId=9394801

	public class MvcApplication : HttpApplication
	{
		protected void Application_Start()
		{
			var engine = MvcEngine.Create();

			ViewEngines.Engines.Insert(0, new ThemedMasterViewEngine());

			engine.RegisterControllers(typeof (StartController).Assembly);

			RegisterRoutes(RouteTable.Routes, engine);
		}

		public static void RegisterRoutes(RouteCollection routes, IEngine engine)
		{
			AreaRegistration.RegisterAllAreas(new AreaRegistrationState(engine));

			routes.MapContentRoute("Content", engine);

			routes.MapRoute(
				"Default", // Route name
				"{controller}/{action}/{*remainder}", // URL with parameters
				new { action = "Index", remainder = "" } // Parameter defaults
				);
		}
	}
}