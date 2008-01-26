using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using N2.Integrity;
using System.Collections.Generic;

namespace N2.Templates.Search.Items
{
	[Definition("Database Search", "DatabaseSearch", "Searches for items searching for texts in the database.", "", 200)]
	public class DatabaseSearch : AbstractSearch
	{
		public virtual Persistence.Finder.IQueryEnding CreateQuery(string query)
		{
			List<Collections.ItemFilter> filters = GetFilters();
			string like = '%' + query + '%';
			return Find.Items
				.Where.Title.Like(like)
				.Or.Name.Like(like)
				.Or.Detail().Like(like)
				.Filters(filters);
		}

		public override ICollection<ContentItem> Search(string query)
		{
			return CreateQuery(query).Select();
		}
	}
}
