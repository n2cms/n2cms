using System.Collections.Generic;

namespace N2.Collections
{
    /// <summary>
    /// Wraps and inverses another filter.
    /// </summary>
    public class InverseFilter : ItemFilter
    {
        private ItemFilter filterToInverse;

        public InverseFilter(ItemFilter filterToInverse)
        {
            this.filterToInverse = filterToInverse;
        }

        public override bool Match(ContentItem item)
        {
            return !filterToInverse.Match(item);
        }

        public static void FilterInverse(IList<ContentItem> items, ItemFilter filterToInverse)
        {
            Filter(items, new InverseFilter(filterToInverse));
        }
    }
}
