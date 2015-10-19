using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Definitions
{
    /// <summary>
    /// The order children are sorted.
    /// </summary>
	public enum SortBy
	{
		/// <summary>Do not reorder children, children appears in sort order but this order is not altered. This can improve performance with large numbers of children.</summary>
		Unordered = 0,
		/// <summary>Children are re-ordered by title in alphabetical ascending order.</summary>
		Title = 1,
		/// <summary>Children are re-ordered by published date in ascending order.</summary>
		Published = 2,
		/// <summary>Children are re-ordered by changed date in ascending order.</summary>
		Updated = 3,
		/// <summary>Children are re-ordered by published date in descending order.</summary>
		PublishedDescending = 4,
		/// <summary>Children are re-ordered by changed date in descending order.</summary>
		UpdatedDescending = 5,
		/// <summary>Children are re-ordered by an expression, e.g. "Title DESC".</summary>
		Expression = 6,
		/// <summary>Children are given an ascending sort index in the order they appear in the children collection. This typically means new children are appended last.</summary>
		CurrentOrder = 7,
		/// <summary>Children are appended last and are given an incrementing sort order.</summary>
		Append = 8,
		/// <summary>A custom sort type is used.</summary>
		CustomSorter = 9
	}
}
