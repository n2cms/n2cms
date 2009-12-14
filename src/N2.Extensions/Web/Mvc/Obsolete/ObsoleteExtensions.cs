using System;
using N2.Web.UI;
using N2.Web.UI.WebControls;
using System.Web;
using System.Web.Mvc;

namespace N2.Web.Mvc.Html
{
	public static class ObsoleteExtensions
    {
        [Obsolete("Use Html.Detail")]
        public static string Detail<TItem>(this IItemContainer container, string detailName)
            where TItem : ContentItem
        {
            return Convert.ToString(container.CurrentItem[detailName]);
        }

        /// <summary>
        /// Renders all items in the Zone of the given name from the item held by the <see cref="container" />.
        /// </summary>
        /// <typeparam name="TItem"></typeparam>
        /// <param name="container"></param>
        /// <param name="zoneName"></param>
        /// <returns></returns>
        [Obsolete("Use Html.Zone")]
        public static ZoneHelper Zone<TItem>(this IItemContainer<TItem> container, string zoneName)
            where TItem : ContentItem
        {
            return container.Zone(zoneName, container.CurrentItem);
        }

        /// <summary>
        /// Renders all items in the Zone of the given name from the <see cref="item" /> given.
        /// </summary>
        /// <typeparam name="TItem"></typeparam>
        /// <param name="container"></param>
        /// <param name="zoneName"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        [Obsolete("Use Html.Display")]
        public static ZoneHelper Zone<TItem>(this IItemContainer<TItem> container, string zoneName, ContentItem item)
            where TItem : ContentItem
        {
            return new ZoneHelper(container.ViewContext(), zoneName, item);
        }

		/// <summary>
		/// Renders all items in the Zone of the given name from the item held by the <see cref="container" />.
		/// </summary>
		/// <remarks>This extension method at the moment does not implement the drag & drop functionality.</remarks>
		/// <param name="container"></param>
		/// <param name="zoneName"></param>
		/// <returns></returns>
        [Obsolete("Use Html.DroppableZone")]
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
		[Obsolete("Use Html.DroppableZone")]
        public static DroppableZoneHelper DroppableZone(this IItemContainer container, string zoneName, ContentItem item)
		{
            return new DroppableZoneHelper(container.ViewContext(), zoneName, item);
        }
	}
}