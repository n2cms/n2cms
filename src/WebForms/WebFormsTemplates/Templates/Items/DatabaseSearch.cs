using System;
using System.Collections.Generic;

namespace N2.Templates.Items
{
    [PageDefinition("Database Search", 
        Description = "Searches for items searching for texts in the database.",
        SortOrder = 200,
        IconUrl = "~/Templates/UI/Img/zoom.png")]
    public class DatabaseSearch : AbstractSearch
    {
        [Obsolete("Text search is now used")]
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

        [Obsolete("Text search is now used")]
        public override ICollection<ContentItem> Search(string query)
        {
            return CreateQuery(query).Select();
        }
    }
}
