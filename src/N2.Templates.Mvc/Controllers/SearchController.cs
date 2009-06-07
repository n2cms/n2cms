using System;
using System.Web.Mvc;
using N2.Templates.Mvc.Items.Pages;
using N2.Templates.Mvc.Models;
using N2.Web;
using N2.Web.Mvc;

namespace N2.Templates.Mvc.Controllers
{
	[Controls(typeof(AbstractSearch))]
	public class SearchController : ContentController<AbstractSearch>
	{
		[NonAction]
		public override ActionResult Index()
		{
			return Index(null);
		}

		public ActionResult Index(string q)
		{
			if (String.IsNullOrEmpty(q))
				return View(new SearchModel(CurrentItem, new ContentItem[0]));

			var hits = CurrentItem.Search(q);

			return View(new SearchModel(CurrentItem, hits) {SearchTerm = q});
		}
	}
}