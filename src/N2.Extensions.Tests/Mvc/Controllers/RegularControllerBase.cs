using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Extensions.Tests.Mvc.Models;
using N2.Web;
using N2.Web.Mvc;
using System.Web.Mvc;

namespace N2.Extensions.Tests.Mvc.Controllers
{
	[Controls(typeof(RegularPage))]
	public class RegularControllerBase : ContentController<RegularPage>
	{
		public override System.Web.Mvc.ActionResult Index()
		{
			throw new NotImplementedException();
		}
	}

	public class SearchController : ContentController<SearchPage>
	{
		public override ActionResult Index()
		{
			throw new NotImplementedException();
		}
		public ActionResult Search(string q)
		{
			return View(new string[q.Length]);
		}
	}
}