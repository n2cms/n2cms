using System;
using System.Collections.Generic;
using System.Linq;
using N2.Persistence.Search;
using N2.Definitions;

namespace N2.Templates.Mvc.Models.Pages
{
    [PageDefinition("Database Search",
        Description = "Searches for items searching for texts on the pages.",
        SortOrder = 200,
        IconClass = "fa fa-search")]
    public class DatabaseSearch : SearchBase
    {
        [Obsolete("Text search is now used from the controller")]
        public override ICollection<ContentItem> Search(string query, int pageNumber, int pageSize, out int totalRecords)
        {
            var q = Query.For(query).Below(SearchRoot).Range(pageSize * pageNumber, pageSize).Pages(true).Except(Query.For(typeof(ISystemNode)));
            var result = Context.Current.Resolve<IContentSearcher>()
                .Search(q);
            
            totalRecords = result.Total;
            return result.Hits.Select(h => h.Content)
                .Where(Content.Is.All(GetFilters()).Match)
                .ToList();
        }
    }
}
