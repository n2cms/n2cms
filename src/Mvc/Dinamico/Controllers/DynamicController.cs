using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using N2.Web.Mvc;
using N2.Web;

namespace Dinamico.Controllers
{
	[Controls(typeof(Models.DynamicPage))]
    public class DynamicController : ContentController
    {
        //
        // GET: /Dynamic/

        public override ActionResult Index()
        {
            return View(CurrentItem);
        }

    }
}
