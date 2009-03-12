using N2.Collections;
using N2.Templates.Items;
using N2.Web;
using N2.Web.Parts;

namespace N2.Templates.Services
{
	/// <summary>
	/// Implements "Recusive" zones functionality.
	/// </summary>
	[Controls(typeof(AbstractPage))]
	public class TemplatesPartsAspectController : PartsAspectController
	{
		public override ItemList GetItemsInZone(ContentItem parentItem, string zoneName)
		{
			ItemList items =  base.GetItemsInZone(parentItem, zoneName);
			ContentItem grandParentItem = parentItem;
			if (zoneName.StartsWith("Recursive") && grandParentItem is AbstractContentPage && !(grandParentItem is LanguageRoot))
			{
				items.AddRange(GetItemsInZone(parentItem.Parent, zoneName));
			}
			return items;
		}
	}
}
