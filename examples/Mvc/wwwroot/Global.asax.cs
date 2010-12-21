using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

using N2.Web.Mvc;
using N2.Engine;

namespace MvcTest
{
	public class GlobalApplication : System.Web.HttpApplication
	{
		public static void RegisterRoutes(RouteCollection routes, IEngine engine)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            
			// This route detects content item paths and executes their controller
			routes.Add(new ContentRoute(engine));
            
			// This controller fallbacks to a controller unrelated to N2
			routes.MapRoute(
               "Default",                                              // Route name
               "{controller}/{action}/{id}",                           // URL with parameters
               new { controller = "Home", action = "Index", id = UrlParameter.Optional }  // Parameter defaults
            );
        }

		protected void Application_Start(object sender, EventArgs e)
		{
			// normally the engine is initialized by the initializer module but it can also be initialized this programmatically
			// since we attach programmatically we need to associate the event broker with a http application

			var engine = MvcEngine.Create();

			engine.RegisterControllers(typeof(GlobalApplication).Assembly);

			RegisterRoutes(RouteTable.Routes, engine);
		}

		public override void Init()
		{
			EventBroker.Instance.Attach(this);
			base.Init();
		}
	}
}