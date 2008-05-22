using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Collections
{
	/// <summary>
	/// The abstract base class of item filters.
	/// </summary>
	public abstract class ItemFilter
	{
		/// <summary>Matches an item against the current filter.</summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public abstract bool Match(ContentItem item);
		
		/// <summary>Filters items by matching them against the current filter.</summary>
		/// <param name="items">The items to perform the filtering on.</param>
		public virtual void Filter(IList<ContentItem> items)
		{
			for (int i = items.Count - 1; i >= 0; i--)
				if (!Match(items[i]))
					items.RemoveAt(i);
		}

		/// <summary>Applies a filter on a list of items.</summary>
		/// <param name="items">The items to filter.</param>
		/// <param name="filter">The filter to apply.</param>
		public static void Filter(IList<ContentItem> items, ItemFilter filter)
		{
			filter.Filter(items);
		}
	}
}
