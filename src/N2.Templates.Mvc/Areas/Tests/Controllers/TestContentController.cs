#if DEBUG
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using N2.Web;
using N2.Web.Mvc;
using N2.Templates.Mvc.Areas.Tests.Models;

namespace N2.Templates.Mvc.Areas.Tests.Controllers
{
	[Controls(typeof(TestPage))]
	[Controls(typeof(TestPart))]
	public class TestContentController : ContentController<ContentItem>
    {
        //
        // GET: /Tests/TestContent/

        public override ActionResult Index()
        {
			if ("Tests" != RouteData.DataTokens["area"])
				throw new Exception("Incorrect area: " + RouteData.Values["area"]);

			if (CurrentItem.IsPage)
				return View();
			else
				return PartialView("Partial");
        }

		[HttpPost]
		public ActionResult Add(string name)
		{
			var part = Engine.Definitions.CreateInstance<TestPart>(CurrentItem);
			part.Name = name;
			part.Title = name;
			part.ZoneName = "TestParts";

			Engine.Persister.Save(part);

			return View("Index");
		}

		public ActionResult Remove()
		{
			Engine.Persister.Delete(CurrentItem);
			return RedirectToParentPage();
		}
    }
}
#endif