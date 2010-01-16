using System;
using System.Web.Mvc;
using N2.Templates.Mvc.Models.Pages;
using N2.Templates.Mvc.Models;
using N2.Web;
using N2.Web.Mvc;

namespace N2.Templates.Mvc.Controllers
{
	[Controls(typeof(AbstractSearch))]
	public class SearchController : ContentController<AbstractSearch>
	{
		private const int PAGE_SIZE = 10;

		[NonAction]
		public override ActionResult Index()
		{
			return Index(null, null);
		}

		public ActionResult Index(string q, int? p)
		{ 
			if (String.IsNullOrEmpty(q))
				return View(new SearchModel(new ContentItem[0]));

			int totalRecords;
			var hits = CurrentItem.Search(q, p ?? 0, PAGE_SIZE, out totalRecords);

			return View(new SearchModel(hits) {SearchTerm = q, TotalResults = totalRecords, PageSize = PAGE_SIZE, PageNumber = p ?? 0});
		}
	}
}