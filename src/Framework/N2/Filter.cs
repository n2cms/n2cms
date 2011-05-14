using System.Collections.Generic;
using System.Linq;
using N2.Collections;

namespace N2
{
	/// <summary>
	/// Provides access to common filters.
	/// </summary>
	public static class Filter
	{
		public static FilterHelper Is
		{
			get { return new FilterHelper(); }
		}

		public static ItemFilter And(this ItemFilter first, params ItemFilter[] alsoRequired)
		{
			return Is.All(first, alsoRequired);
		}

		public static ItemFilter Or(this ItemFilter first, params ItemFilter[] alternativeOptions)
		{
			return Is.Any(first, alternativeOptions);
		}

		public static IEnumerable<ContentItem> Where(this IEnumerable<ContentItem> items, ItemFilter filter)
		{
			return items.Where(filter.Match);
		}
	}
}
