using N2.Web;
using System.Linq;
using System.Collections.Generic;

namespace N2.Tests.Web.Items
{
	[Controls(typeof(PageItem))]
	public class PageZoneController : ZoneAspectController
	{
		public override IEnumerable<ContentItem> GetItemsInZone(ContentItem parentItem, string zoneName)
		{
			if(zoneName.EndsWith("None"))
				return Enumerable.Empty<ContentItem>();
			if (zoneName.EndsWith("All"))
				return parentItem.GetChildren().Where(ci => ci.ZoneName != null);

			return parentItem.GetChildren(zoneName);
		}
	}
}