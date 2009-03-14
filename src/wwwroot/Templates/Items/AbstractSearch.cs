using System.Collections.Generic;
using N2.Collections;
using N2.Details;
using N2.Integrity;

namespace N2.Templates.Items
{
    [RestrictParents(typeof (IStructuralPage))]
	[DefaultTemplate("Search")]
    public abstract class AbstractSearch : AbstractContentPage
    {
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
                filters.Add(new ParentFilter(SearchRoot));
            return filters;
        }

        protected override string IconName
        {
            get { return "zoom"; }
        }
    }
}