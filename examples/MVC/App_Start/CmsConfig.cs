using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using N2.Web.Mvc;
using N2.Engine;

namespace MVC
{
    public class CmsConfig
    {
        public static void Register(RouteCollection routes, ViewEngineCollection viewEngines, IEngine engine)
        {
            routes.MapContentRoute("Content", engine);
        }
    }
}
