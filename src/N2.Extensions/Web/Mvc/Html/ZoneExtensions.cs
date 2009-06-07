using System;

namespace N2.Web.Mvc.Html
{
	public static class ZoneExtensions
	{
		public static ZoneHelper Zone<TItem>(this IItemContainer<TItem> container, string zoneName)
			where TItem : ContentItem
		{
			return container.Zone(zoneName, container.CurrentItem);
		}

		public static ZoneHelper Zone<TItem>(this IItemContainer<TItem> container, string zoneName, ContentItem item)
			where TItem : ContentItem
		{
			return new ZoneHelper(container, zoneName, item);
		}
	}
}