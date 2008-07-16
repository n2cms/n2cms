using System;
using System.Collections.Generic;
using System.Web;
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
			IRouteHandler routeHandler = new MvcRouteHandler();
			
			routes.Add(new ContentRoute(engine, routeHandler));
            
			routes.MapRoute("Default", "{controller}/{action}/{id}", new { action = "Index" }, new { controller = @"[^\.]*" });
		}

		protected void Application_Start(object sender, EventArgs e)
		{
			N2.Context.Initialize(false);
			RegisterRoutes(RouteTable.Routes, N2.Context.Current);
		}
	}
}