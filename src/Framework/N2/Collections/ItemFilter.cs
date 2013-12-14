using System;
using System.Collections.Generic;

namespace N2.Collections
{
    /// <summary>
    /// The abstract base class of item filters.
    /// </summary>
    public abstract class ItemFilter : IPipeline<ContentItem>, IDisposable
    {
        /// <summary>Matches an item against the current filter.</summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public abstract bool Match(ContentItem item);
        
        /// <summary>Filters items by matching them against the current filter.</summary>
        /// <param name="items">The items to perform the filtering on.</param>
        public virtual void Filter(IList<ContentItem> items)
        {
            using (this)
            {
                for (int i = items.Count - 1; i >= 0; i--)
                    if (!Match(items[i]))
                        items.RemoveAt(i);
            }
        }

        /// <summary>Filters the supplied items according to the current filter type.</summary>
        /// <param name="items">The initial item enumeration.</param>
        /// <returns>The filtered item enumeration.</returns>
        public virtual IEnumerable<T> Pipe<T>(IEnumerable<T> items)
            where T : ContentItem
        {
            using (this)
            {
                foreach (T item in items)
                    if (Match(item))
                        yield return item;
            }
        }

        /// <summary>Filters the supplied items according to the current filter type.</summary>
        /// <param name="items">The initial item enumeration.</param>
        /// <returns>The filtered item enumeration.</returns>
        public virtual IEnumerable<ContentItem> Pipe(IEnumerable<ContentItem> items)
        {
            return Pipe<ContentItem>(items);
        }
        
        /// <summary>Applies a filter on a list of items.</summary>
        /// <param name="items">The items to filter.</param>
        /// <param name="filter">The filter to apply.</param>
        public static void Filter(IList<ContentItem> items, ItemFilter filter)
        {
            filter.Filter(items);
        }

        #region IDisposable Members

        /// <summary>Resets the filter if applicable.</summary>
        public virtual void Dispose()
        {
            // do nothing
        }

        #endregion

        #region implicit operator Func<ContentItem, bool>
        public static implicit operator Func<ContentItem, bool>(ItemFilter filter)
        {
            if (filter == null)
                return (ci) => true;
            return filter.Match;
        }

        public static implicit operator ItemFilter(Func<ContentItem, bool> isPositiveMatch)
        {
            if (isPositiveMatch == null)
                return new NullFilter();
            return new DelegateFilter(isPositiveMatch);
        }
        #endregion

        #region Operators

        public static ItemFilter operator &(ItemFilter f1, ItemFilter f2)
        {
            return new AllFilter(f1, f2);
        }

        public static ItemFilter operator |(ItemFilter f1, ItemFilter f2)
        {
            return new DelegateFilter(i => f1.Match(i) || f2.Match(i));
        }

        public static ItemFilter operator +(ItemFilter f1, ItemFilter f2)
        {
            return new AllFilter(f1, f2);
        }

        public static ItemFilter operator -(ItemFilter f1, ItemFilter f2)
        {
            return new AllFilter(f1, new InverseFilter(f2));
        }

        #endregion
    }
}
