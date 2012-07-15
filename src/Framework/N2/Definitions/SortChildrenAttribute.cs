using System;
using System.Collections.Generic;
using System.Linq;
using N2.Collections;
using N2.Persistence.Behaviors;

namespace N2.Definitions
{
	/// <summary>
	/// Controls the order of children added to items decorated with this attribute.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class SortChildrenAttribute : Attribute, ISavingBehavior
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
			// Possible workaround of issue lazy-loading children 
			// List<T> takes some shortcuts that might bypass nh bag initialization
			ItemList temp = new ItemList(item.Children.Select(c => c));
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

		void ISavingBehavior.OnSaving(BehaviorContext context)
		{
		}

		public void OnSavingChild(BehaviorContext context)
		{
            if (OrderBy == SortBy.Unordered)
                return;

            var item = context.AffectedItem;

            if (item.Parent == null)
				return;
            
            if (item.ID == 0)
                item.AddTo(item.Parent);

			foreach (ContentItem updatedItem in ReorderChildren(item.Parent))
			{
				context.UnsavedItems.Add(updatedItem);
			}
		}
	}
}
