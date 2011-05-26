using System;
using System.Linq;
using System.Web.Mvc;
using N2.Templates.Mvc.Models.Pages;
using N2.Templates.Mvc.Models;
using N2.Web;
using N2.Web.Mvc;
using N2.Persistence;

namespace N2.Templates.Mvc.Controllers
{
	[Controls(typeof(SearchBase))]
	public class SearchController : ContentController<SearchBase>
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
			int skip = (p ?? 0) * PAGE_SIZE;
			int take = PAGE_SIZE;
			var hits = Engine.Resolve<ITextSearcher>().Search(CurrentItem.SearchRoot, q, skip, take, out totalRecords)
				.Select(i => i.IsPage ? i : Find.ClosestPage(i))
				.Distinct()
				.Where(Filter.Is.AccessiblePage().Match)
				.ToList();

			return View(new SearchModel(hits) {SearchTerm = q, TotalResults = totalRecords, PageSize = PAGE_SIZE, PageNumber = p ?? 0});
		}
	}
}