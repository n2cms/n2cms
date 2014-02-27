#if DEBUG
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using N2.Web.Mvc;

namespace N2.Templates.Mvc.Areas.Tests.Controllers
{
    /// <summary>
    /// This is an example of a controller external to n2. It's not routed through the 
    /// content route. The [ExternalContent] attribute automatically maps a content
    /// item to this controller using the "action" route parameter as key. This allows
    /// the usage of parts and exiting content from the view.
    /// </summary>
    [ExternalContent("action", ContentType = typeof(N2.Templates.Mvc.Models.Pages.TextPage))]
    public class ExternalTypedController : Controller
    {
        //
        // GET: /Tests/Static/  

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
#endif
