using System.Collections.Generic;
using System.Linq;
using N2.Collections;
using N2.Persistence.Finder;

namespace N2.Templates.Mvc.Models.Pages
{
	[PageDefinition("Database Search",
		Description = "Searches for items searching for texts in the database.",
		SortOrder = 200,
		IconUrl = "~/Content/Img/zoom.png")]
	public class DatabaseSearch : SearchBase
	{
		public virtual IQueryEnding CreateQuery(string query)
		{
			string like = '%' + query + '%';

			var q = Find.Items.Where
				.OpenBracket()
					.Title.Like(like)
					.Or.Name.Like(like)
					.Or.Detail().Like(like)
				.CloseBracket()
				.And.ZoneName.IsNull(true);

			if (SearchRoot != null)
				q = q.And.AncestralTrail.Like(Utility.GetTrail(SearchRoot) + "%");

			return q;
		}

		public override ICollection<ContentItem> Search(string query, int pageNumber, int pageSize, out int totalRecords)
		{
			var q = CreateQuery(query);
			totalRecords = q.Count();

			return q.FirstResult(pageSize*pageNumber)
				.MaxResults(pageSize)
				.Filters(GetFilters())
				.Select();
		}
	}
}