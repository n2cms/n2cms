using System.Collections.Generic;
using System.Linq;

namespace N2.Templates.Mvc.Items.Pages
{
	[PageDefinition("Database Search",
		Description = "Searches for items searching for texts in the database.",
		SortOrder = 200,
		IconUrl = "~/Content/Img/zoom.png")]
	public class DatabaseSearch : AbstractSearch
	{
		public virtual Persistence.Finder.IQueryEnding CreateQuery(string query)
		{
			List<Collections.ItemFilter> filters = GetFilters();
			string like = '%' + query + '%';

			return Mvc.Find.Items
				.Where.Title.Like(like)
				.Or.Name.Like(like)
				.Or.Detail().Like(like)
				.Filters(filters);
		}

		public override ICollection<ContentItem> Search(string query, int pageNumber, int pageSize, out int totalRecords)
		{
			var n2Query = CreateQuery(query);

			var records = n2Query.Select();

			totalRecords = records.Count;

			return n2Query
				.Select()
				.Skip(pageSize*pageNumber)
				.Take(pageSize)
				.ToList();
		}
	}
}