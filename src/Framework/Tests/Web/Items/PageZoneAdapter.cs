using N2.Collections;
using N2.Engine;
using N2.Web.Parts;

namespace N2.Tests.Web.Items
{
	[Adapts(typeof(PageItem))]
	public class PageZoneAdapter : PartsAdapter
	{
		public override ItemList GetItemsInZone(ContentItem parentItem, string zoneName)
		{
			if(zoneName.EndsWith("None"))
				return new ItemList();
			if (zoneName.EndsWith("All"))
				return parentItem.GetChildren(new DelegateFilter(ci => ci.ZoneName != null));

			return parentItem.GetChildren(zoneName);
		}
	}
}