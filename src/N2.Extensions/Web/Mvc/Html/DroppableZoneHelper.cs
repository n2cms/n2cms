using System;
using N2.Web.UI;

namespace N2.Web.Mvc.Html
{
	public class DroppableZoneHelper : ZoneHelper
	{
		public DroppableZoneHelper(IItemContainer container, string zoneName)
			: base(container, zoneName)
		{
		}

		public DroppableZoneHelper(IItemContainer container, string zoneName, ContentItem item)
			: base(container, zoneName, item)
		{
		}

		public DroppableZoneHelper AllowExternalManipulation()
		{
			// TODO: Not implemented
			return this;
		}
	}
}