using System;
using System.Collections.Generic;
using System.Linq;
using N2.Persistence.Search;

namespace N2.Templates.Mvc.Models.Pages
{
	[PageDefinition("Database Search",
		Description = "Searches for items searching for texts on the pages.",
		SortOrder = 200,
		IconUrl = "~/Content/Img/zoom.png")]
	public class DatabaseSearch : SearchBase
	{
		[Obsolete("Text search is now used from the controller")]
		public override ICollection<ContentItem> Search(string query, int pageNumber, int pageSize, out int totalRecords)
		{
			var result = Context.Current.Resolve<ITextSearcher>()
				.Search(SearchQuery.For(query).Below(SearchRoot).Range(pageSize * pageNumber, pageSize).Pages(true));
			
			totalRecords = result.Total;
			return result.Hits.Select(h => h.Content)
				.Where(Filter.Is.All(GetFilters()).Match)
				.ToList();
		}
	}
}