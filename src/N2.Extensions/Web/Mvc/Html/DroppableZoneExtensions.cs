using System;
using N2.Web.UI;

namespace N2.Web.Mvc.Html
{
	public static class DroppableZoneExtensions
	{
		/// <summary>
		/// Renders all items in the Zone of the given name from the item held by the <see cref="container" />.
		/// </summary>
		/// <remarks>This extension method at the moment does not implement the drag & drop functionality.</remarks>
		/// <param name="container"></param>
		/// <param name="zoneName"></param>
		/// <returns></returns>
		public static DroppableZoneHelper DroppableZone(this IItemContainer container, string zoneName)
		{
			return container.DroppableZone(zoneName, container.CurrentItem);
		}

		/// <summary>
		/// Renders all items in the Zone of the given name from the <see cref="item" /> given.
		/// </summary>
		/// <remarks>This extension method at the moment does not implement the drag & drop functionality.</remarks>
		/// <param name="container"></param>
		/// <param name="zoneName"></param>
		/// <returns></returns>
		public static DroppableZoneHelper DroppableZone(this IItemContainer container, string zoneName, ContentItem item)
		{
			return new DroppableZoneHelper(container, zoneName, item);
		}
	}
}