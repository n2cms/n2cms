using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using N2.Web;
using N2.Web.Mvc;
using Dinamico.Models;

namespace Dinamico.Controllers
{
	[Controls(typeof(StartPage))]
	public class StartPageController : ContentController
    {
		public ActionResult NotFound()
		{
			return View();
		}
    }
}
