using System;
using N2.Web.UI;
using N2.Web.UI.WebControls;
using System.Web;

namespace N2.Web.Mvc.Html
{
	public static class DroppableZoneExtensions
	{
        public static ControlPanelState ControlPanelState(this HttpContextBase context)
        {
            return ControlPanel.GetState(context.User, context.Request.QueryString);
        }

        /// <summary>
        /// Renders all items in the Zone of the given name from the item held by the <see cref="container" />.
        /// </summary>
        /// <remarks>This extension method at the moment does not implement the drag & drop functionality.</remarks>
        /// <param name="container"></param>
        /// <param name="zoneName"></param>
        /// <returns></returns>
        public static DroppableZoneHelper DroppableZone(this System.Web.Mvc.HtmlHelper helper, string zoneName)
        {
            return new DroppableZoneHelper(helper.ViewContext, zoneName);
        }

        /// <summary>
        /// Renders all items in the Zone of the given name from the <see cref="item" /> given.
        /// </summary>
        /// <remarks>This extension method at the moment does not implement the drag & drop functionality.</remarks>
        /// <param name="container"></param>
        /// <param name="zoneName"></param>
        /// <returns></returns>
        public static DroppableZoneHelper DroppableZone(this System.Web.Mvc.HtmlHelper helper, string zoneName, ContentItem item)
        {
            return new DroppableZoneHelper(helper.ViewContext, zoneName, item);
        }






		/// <summary>
		/// Renders all items in the Zone of the given name from the item held by the <see cref="container" />.
		/// </summary>
		/// <remarks>This extension method at the moment does not implement the drag & drop functionality.</remarks>
		/// <param name="container"></param>
		/// <param name="zoneName"></param>
		/// <returns></returns>
        [Obsolete("Prefer Html.DroppableZone")]
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
		[Obsolete("Prefer Html.DroppableZone")]
        public static DroppableZoneHelper DroppableZone(this IItemContainer container, string zoneName, ContentItem item)
		{
            return new DroppableZoneHelper(container.ViewContext(), zoneName, item);
        }
	}
}