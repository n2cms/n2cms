using System;
using System.Collections.Generic;
using N2.Collections;
using N2.Definitions;
using N2.Details;
using N2.Integrity;
using N2.Web;

namespace N2.Templates.Items
{
    [RestrictParents(typeof (IStructuralPage))]
    [ConventionTemplate("Search")]
    public abstract class AbstractSearch : AbstractContentPage
    {
        [Obsolete("Text search is now used")]
        public abstract ICollection<ContentItem> Search(string query);

        [EditableLink("Search Root", 100, ContainerName = Tabs.Content)]
        public virtual ContentItem SearchRoot
        {
            get { return (ContentItem) GetDetail("SearchRoot"); }
            set { SetDetail("SearchRoot", value); }
        }

        protected virtual List<ItemFilter> GetFilters()
        {
            List<ItemFilter> filters = new List<ItemFilter>();
            filters.Add(new NavigationFilter());
            if (SearchRoot != null)
                filters.Add(new AncestorFilter(SearchRoot));
            return filters;
        }
    }
}
