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
    /// item to this controller using the "id" route parameter as key. This allows
    /// the usage of parts.
    /// </summary>
    [ExternalContent("id")]
    public class ExternalController : Controller
    {
        //
        // GET: /Tests/Static/  

        public ActionResult Index(string id)
        {
            return View();
        }

    }
}
#endif
