using System;
using System.Collections.Generic;
using System.Text;
using N2.Engine.Aspects;

namespace N2.Web
{
	/// <summary>
	/// Controls aspects related to zones, zone definitions, and items to display in a zone.
	/// </summary>
	public class ZoneAspectController : IAspectController
	{
		#region IAspectController Members

		public PathData Path { get; set; }

		public N2.Engine.IEngine Engine { get; set; }

		#endregion

		public virtual IEnumerable<ContentItem> GetItemsInZone(ContentItem parentItem, string zoneName)
		{
			return parentItem.GetChildren(zoneName);
		}
	}
}
