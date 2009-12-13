using System;
using System.Web.Mvc;

namespace N2.Web.Mvc.Html
{
	public static class ZoneExtensions
	{
        /// <summary>
        /// Renders all items in the Zone of the given name from the item held by the <see cref="container" />.
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="zoneName"></param>
        /// <returns></returns>
        public static ZoneHelper Zone(HtmlHelper helper, string zoneName)
        {
            return new ZoneHelper(helper.ViewContext, zoneName);
        }

        /// <summary>
        /// Renders all items in the Zone of the given name from the <see cref="item" /> given.
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="zoneName"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static ZoneHelper Zone(HtmlHelper helper, string zoneName, ContentItem item)
        {
            return new ZoneHelper(helper.ViewContext, zoneName, item);
        }





		/// <summary>
		/// Renders all items in the Zone of the given name from the item held by the <see cref="container" />.
		/// </summary>
		/// <typeparam name="TItem"></typeparam>
		/// <param name="container"></param>
		/// <param name="zoneName"></param>
		/// <returns></returns>
        [Obsolete("Prefer Html.Display")]
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
        [Obsolete("Prefer Html.Display")]
        public static ZoneHelper Zone<TItem>(this IItemContainer<TItem> container, string zoneName, ContentItem item)
			where TItem : ContentItem
		{
			return new ZoneHelper(container.ViewContext(), zoneName, item);
		}
	}
}