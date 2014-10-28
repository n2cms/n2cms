using System;
using System.Collections.Generic;

namespace N2.Collections
{
    /// <summary>
    /// A list of content items. Provides easier access to filtering and sorting.
    /// </summary>
    public class ItemList : ItemList<ContentItem>
    {
        /// <summary>Initializes an empty instance of the ItemList class.</summary>
        public ItemList()
        {
        }

        /// <summary>Initializes an instance of the ItemList class with the supplied items.</summary>
        public ItemList(IEnumerable<ContentItem> items)
            : base(items)
        {
        }

        /// <summary>Initializes an instance of the ItemList class with the supplied items.</summary>
        public ItemList(Func<IEnumerable<ContentItem>> itemsFactory)
            : base(itemsFactory)
        {
        }

        /// <summary>Initializes an instance of the ItemList class adding the items matching the supplied filter.</summary>
        /// <param name="items">The gross enumeration of items to initialize with.</param>
        /// <param name="filter">The filters that should be applied to the gross collection.</param>
        public ItemList(IEnumerable<ContentItem> items, ItemFilter filter)
            : base(filter.Pipe(items))
        {
        }

    }
}
