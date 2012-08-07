#if DEBUG
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using N2.Web;
using N2.Web.Mvc;
using N2.Templates.Mvc.Areas.Tests.Models;
using N2.Templates.Mvc.Models.Pages;
using N2.Definitions;
using N2.Persistence;

namespace N2.Templates.Mvc.Areas.Tests.Controllers
{
	[Controls(typeof(TestPage))]
	public class TestPageController : ContentController<ContentItem>
    {
		IDefinitionManager definitions;
		ContentActivator activator;

		public TestPageController(IDefinitionManager definitions, ContentActivator activator)
		{
			this.definitions = definitions;
			this.activator = activator;
		}

		public override ActionResult Index()
		{
			if ("Tests" != (string)RouteData.DataTokens["area"])
				throw new Exception("Incorrect area: " + RouteData.Values["area"]);

			return View();
		}

		public ActionResult Test()
		{
			return View();
		}

		public ActionResult TheAction()
		{
			return Content("TheAction");
		}
    }
}
#endif