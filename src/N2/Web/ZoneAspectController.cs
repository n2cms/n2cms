using System.Collections.Generic;
using N2.Collections;
using N2.Engine.Aspects;
using N2.Definitions;

namespace N2.Web
{
	/// <summary>
	/// Controls aspects related to zones, zone definitions, and items to display in a zone.
	/// </summary>
	[Controls(typeof(ContentItem))]
	public class ZoneAspectController : IAspectController
	{
		#region IAspectController Members

		public PathData Path { get; set; }

		public N2.Engine.IEngine Engine { get; set; }

		#endregion

		public virtual ItemList GetItemsInZone(ContentItem parentItem, string zoneName)
		{
			return parentItem.GetChildren(zoneName);
		}

		public IEnumerable<ItemDefinition> GetDefinitions(ContentItem contentItem, string zone)
		{
			foreach(ItemDefinition definition in N2.Context.Definitions.GetDefinitions())
			{
				if (definition.IsAllowedInZone(zone))
				{
					yield return definition;
				}
			}
		}
	}
}
