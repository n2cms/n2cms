using N2;
using N2.Web.Parts;
using N2.Engine;
using Dinamico.Models;
using System.Linq;

namespace Dinamico.Registrations
{
	/// <summary>
	/// Implements "Recusive" zones functionality.
	/// </summary>
	[Adapts(typeof(ContentPage))]
	public class RecursiveZonesAdapter : PartsAdapter
	{
		public override System.Collections.Generic.IEnumerable<ContentItem> GetParts(ContentItem parentItem, string zoneName, string @interface)
		{
			var items = base.GetParts(parentItem, zoneName, @interface);
			ContentItem grandParentItem = parentItem;
			if (zoneName.StartsWith("Recursive") && grandParentItem is ContentPage && !(grandParentItem is LanguageIntersection))
			{
				if (!parentItem.VersionOf.HasValue)
				{
					items = items.Union(GetParts(parentItem.Parent, zoneName, @interface));
				}
				else
				{
					items = items.Union(GetParts(parentItem.VersionOf.Parent, zoneName, @interface));
				}
			}
			return items;
		}
	}
}