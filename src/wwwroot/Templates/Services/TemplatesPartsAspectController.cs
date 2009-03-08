using N2.Collections;
using N2.Templates.Items;
using N2.Web;
using N2.Web.Parts;

namespace N2.Templates.Services
{
	[Controls(typeof(AbstractPage))]
	public class TemplatesPartsAspectController : PartsAspectController
	{
		public override ItemList GetItemsInZone(ContentItem parentItem, string zoneName)
		{
			ItemList items =  base.GetItemsInZone(parentItem, zoneName);
			if (zoneName.StartsWith("Recursive") && parentItem.Parent is AbstractContentPage)
			{
				items.AddRange(GetItemsInZone(parentItem.Parent, zoneName));
			}
			return items;
		}
	}
}
