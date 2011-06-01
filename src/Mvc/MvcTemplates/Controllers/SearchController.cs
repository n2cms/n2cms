using System;
using System.Linq;
using System.Web.Mvc;
using N2.Templates.Mvc.Models.Pages;
using N2.Templates.Mvc.Models;
using N2.Web;
using N2.Web.Mvc;
using N2.Persistence;
using N2.Persistence.Search;
using System.Web.Security;
using N2.Definitions;

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

			int skip = (p ?? 0) * PAGE_SIZE;
			int take = PAGE_SIZE;
			var query = Query.For(q)
				.Below(CurrentItem.SearchRoot)
				.Range(skip, take)
				.Pages(true)
				.ReadableBy(User, Roles.GetRolesForUser)
				.Except(Query.For(typeof(ISystemNode)));
			var result = Engine.Resolve<ITextSearcher>().Search(query);

			return View(new SearchModel(result.Hits.Select(h => h.Content).Where(Filter.Is.Accessible()).ToList()) { SearchTerm = q, TotalResults = result.Total, PageSize = PAGE_SIZE, PageNumber = p ?? 0 });
		}
	}
}