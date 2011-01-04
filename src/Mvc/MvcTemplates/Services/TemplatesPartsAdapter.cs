using N2.Collections;
using N2.Templates.Mvc.Models.Pages;
using N2.Web;
using N2.Web.Parts;

namespace N2.Templates.Mvc.Services
{
	/// <summary>
	/// Implements "Recusive" zones functionality.
	/// </summary>
	[Controls(typeof(PageBase))]
	public class TemplatesPartsAdapter : PartsAdapter
	{
		public override ItemList GetItemsInZone(ContentItem parentItem, string zoneName)
		{
			ItemList items =  base.GetItemsInZone(parentItem, zoneName);
			ContentItem grandParentItem = parentItem;
			if (zoneName.StartsWith("Recursive") && grandParentItem is ContentPageBase && !(grandParentItem is LanguageRoot))
			{
				if(parentItem.VersionOf == null)
					items.AddRange(GetItemsInZone(parentItem.Parent, zoneName));
				else
					items.AddRange(GetItemsInZone(parentItem.VersionOf.Parent, zoneName));
			}
			return items;
		}
	}
}