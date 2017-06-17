using System;
using System.Collections.Generic;
using System.Linq;
using Dinamico.Models;
using N2;
using N2.Engine;
using N2.Web.Parts;

namespace Dinamico.Registrations
{
	/// <summary>
	///     Implements "Recusive" zones functionality.
	/// </summary>
	[Adapts(typeof(ContentPage))]
	public class RecursiveZonesAdapter : PartsAdapter
	{
		public override IEnumerable<ContentItem> GetParts(ContentItem page, string zoneName, string @interface)
		{
			var items = base.GetParts(page, zoneName, @interface);

			var pageParent = page.VersionOf.HasValue
				? (ContentItem) page.VersionOf.Parent
				: page.Parent;

			if ((pageParent != null) && zoneName.StartsWith("Recursive"))
				return items.Union(GetParts(pageParent, zoneName, @interface));

			return items;
		}
	}
}