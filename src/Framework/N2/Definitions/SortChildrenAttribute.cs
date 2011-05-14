using System;
using System.Collections.Generic;
using System.Linq;
using N2.Collections;

namespace N2.Definitions
{
	/// <summary>
	/// The order children are sorted.
	/// </summary>
	public enum SortBy
	{
		/// <summary>Children are re-ordered by title in alphabetical ascending order.</summary>
		Title,
		/// <summary>Children are re-ordered by published date in ascending order.</summary>
		Published,
		/// <summary>Children are re-ordered by changed date in ascending order.</summary>
		Updated,
		/// <summary>Children are re-ordered by published date in descending order.</summary>
		PublishedDescending,
		/// <summary>Children are re-ordered by changed date in descending order.</summary>
		UpdatedDescending,
		/// <summary>Children are re-ordered by an expression, e.g. "Title DESC".</summary>
		Expression,
		/// <summary>Children are given an ascending sort index in the order they appear in the children collection. This typically means new children are appended last.</summary>
		CurrentOrder,
		/// <summary>Do not reorder children, children appears in sort order but this order is not altered. This can improve performance with large numbers of children.</summary>
		Unordered
	}

	/// <summary>
	/// Controls the order of children added to items decorated with this attribute.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class SortChildrenAttribute : Attribute
	{
		public SortChildrenAttribute(SortBy orderBy)
		{
			this.OrderBy = orderBy;
		}

		/// <summary>Reorders children according to OrderBy.</summary>
		/// <param name="item">The item whose children to re-order.</param>
		/// <returns>Items that were changed in the process of ordering. These items needs to be saved for the order to be persisted.</returns>
		public virtual IEnumerable<ContentItem> ReorderChildren(ContentItem item)
		{
			switch (OrderBy)
			{
				case SortBy.Updated:
					return ReorderBy(item, "Updated");
				case SortBy.UpdatedDescending:
					return ReorderBy(item, "Updated DESC");
				case SortBy.CurrentOrder:
					return Utility.UpdateSortOrder(item.Children);
				case SortBy.Expression:
					return ReorderBy(item, SortExpression);
				case SortBy.Published:
					return ReorderBy(item, "Published");
				case SortBy.PublishedDescending:
					return ReorderBy(item, "Published DESC");
				case SortBy.Title:
					return ReorderBy(item, "Title");
				case SortBy.Unordered:
					return Enumerable.Empty<ContentItem>();
				default:
					throw new ArgumentException("Unknown sort order: " + OrderBy);
			}
		}

		private IEnumerable<ContentItem> ReorderBy(ContentItem item, string sortExpression)
		{
			ItemList temp = new ItemList(item.Children);
			temp.Sort(sortExpression);
			
			item.Children.Clear();
			foreach (var child in temp)
				item.Children.Add(child);

			return Utility.UpdateSortOrder(temp);
		}

		/// <summary>The order to apply to children.</summary>
		public SortBy OrderBy { get; protected set; }

		/// <summary>An expression used to sort children when order by <see cref="SortBy.Expression" /> is defined.</summary>
		public string SortExpression { get; set; }
	}
}
