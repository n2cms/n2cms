using System;

namespace N2.Web.Mvc.Html
{
	public static class DroppableZoneExtensions
	{
		public static DroppableZoneHelper DroppableZone<TItem>(this IItemContainer<TItem> container, string zoneName)
			where TItem : ContentItem
		{
			return container.DroppableZone(zoneName, container.CurrentItem);
		}

		public static DroppableZoneHelper DroppableZone<TItem>(this IItemContainer<TItem> container, string zoneName, ContentItem item)
			where TItem : ContentItem
		{
			return new DroppableZoneHelper(container, zoneName, item);
		}
	}
}