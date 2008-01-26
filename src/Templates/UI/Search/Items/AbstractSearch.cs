using System.Collections.Generic;
using N2.Collections;
using N2.Details;
using N2.Integrity;
using N2.Templates.Items;

namespace N2.Templates.Search.Items
{
	[RestrictParents(typeof (IStructuralPage))]
	public abstract class AbstractSearch : AbstractContentPage
	{
		public abstract ICollection<ContentItem> Search(string query);

		[EditableLink("Search Root", 100)]
		public virtual ContentItem SearchRoot
		{
			get { return (ContentItem) GetDetail("SearchRoot"); }
			set { SetDetail("SearchRoot", value); }
		}

		protected virtual List<ItemFilter> GetFilters()
		{
			List<ItemFilter> filters = new List<ItemFilter>();
			filters.Add(new AccessFilter());
			if (SearchRoot != null)
				filters.Add(new ParentFilter(SearchRoot));
			return filters;
		}

		public override string IconUrl
		{
			get { return "~/Search/UI/Img/zoom.png"; }
		}

		public override string TemplateUrl
		{
			get { return "~/Search/UI/Default.aspx"; }
		}
	}
}