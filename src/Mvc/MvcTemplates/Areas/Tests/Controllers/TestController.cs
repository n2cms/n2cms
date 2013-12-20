#if DEBUG
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace N2.Templates.Mvc.Areas.Tests.Controllers
{
    public class TestController : Controller
    {
        //
        // GET: /Tests/Test/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Test()
        {
            return View();
        }

    }
}
#endif
