using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using N2.Collections;
using N2.Persistence.Finder;
using N2.Templates.Mvc.Items.Items;
using N2.Templates.Mvc.Models;
using N2.Web;
using N2.Web.Mvc;

namespace N2.Templates.Mvc.Controllers
{
	[Controls(typeof (Statistics))]
	public class StatisticsController : ContentController<Statistics>
	{
		public override ActionResult Index()
		{
			var model = new StatisticsModel(CurrentItem);

			IList<ContentItem> items = Find.Items.All.Select();
			int itemsCount = items.Count;
			model.NumberOfItems = itemsCount;

			PageFilter.FilterPages(items);
			model.NumberOfPages = items.Count;

			int totalCount = Find.Items.All.PreviousVersions(VersionOption.Include).Select().Count;
			model.VersionsPerItem = totalCount/(double) itemsCount;

			model.PagesServed = null;
			model.ChangesLastWeek = Find.Items.Where.Updated.Ge(DateTime.Now.AddDays(-7)).Select().Count;
			model.LatestChanges = Find.Items.All.MaxResults(CurrentItem.LatestChangesMaxCount).OrderBy.Updated.Desc.Select()
				.Select(item => FindParentPage(item));

			return View(model);
		}

		private static ContentItem FindParentPage(ContentItem item)
		{
			ContentItem page = item;
			while (page != null && !page.IsPage)
				page = page.Parent;

			return page;
		}
	}
}