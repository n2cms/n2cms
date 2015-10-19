using System;
using System.Collections.Generic;
using System.Linq;
using N2.Collections;
using N2.Persistence.Behaviors;
using N2.Engine;

namespace N2.Definitions
{
    /// <summary>
    /// Controls the order of children added to items decorated with this attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class SortChildrenAttribute : Attribute, ISavingBehavior
    {
		public SortChildrenAttribute(SortBy orderBy)
		{
			this.OrderBy = orderBy;
		}

		public SortChildrenAttribute(Type customSorter)
		{
			if (customSorter != null)
				OrderBy = SortBy.CustomSorter;
			else
				OrderBy = SortBy.CurrentOrder;
			CustomSorterType = customSorter;
		}

		public SortChildrenAttribute(string customSorterType)
			: this(Type.GetType(customSorterType))
		{
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
                case SortBy.Append:
                    return Enumerable.Empty<ContentItem>();
				case SortBy.CustomSorter:
					if (CustomSorterType == null)
					{
						if (item is IChildrenSorter)
							return (item as IChildrenSorter).ReorderChildren(item);
						else
							throw new ArgumentException("Cannot sort by CustomSorter with null CustomSorterType");
					}
					if (!typeof(IChildrenSorter).IsAssignableFrom(CustomSorterType))
						throw new ArgumentException("CustomSorterType must implement IChildrenSorter");
					var sorter = (IChildrenSorter)Services.Service.Resolve(CustomSorterType)
						?? (IChildrenSorter)Activator.CreateInstance(CustomSorterType);
					return sorter.ReorderChildren(item);
                default:
                    throw new ArgumentException("Unknown sort order: " + OrderBy);
            }
        }

		public Accessor<IServiceContainer> Services;

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
            
            foreach (ContentItem updatedItem in ReorderChildren(item.Parent))
            {
                context.UnsavedItems.Add(updatedItem);
            }
        }

		public Type CustomSorterType { get; set; }
	}
}
