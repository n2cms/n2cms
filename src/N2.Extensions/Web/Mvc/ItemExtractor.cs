using System;
using N2.Web.UI;

namespace N2.Web.Mvc
{
	internal static class ItemExtractor
	{
		internal static ContentItem ExtractFromModel(object model)
		{
			var item = model as ContentItem;

			if (item != null)
				return item;

			var itemContainer = model as IItemContainer;

			return itemContainer != null ? itemContainer.CurrentItem : null;
		}
	}
}