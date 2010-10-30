using System.Collections.Generic;
using N2.Collections;
using N2.Details;
using N2.Integrity;
using N2.Web.Mvc;
using N2.Definitions;

namespace N2.Templates.Mvc.Models.Pages
{
	[RestrictParents(typeof (IStructuralPage))]
	public abstract class SearchBase : ContentPageBase
	{
		public abstract ICollection<ContentItem> Search(string query, int pageSize, int pageNumber, out int totalRecords);

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
	}
}