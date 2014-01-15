using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Definitions;

namespace N2.Persistence.Sources
{
    public static class SourcesExtensions
    {

        /// <summary>
        /// Updates sort order so it's sequential for the item and it's siblings and returns items that were updated and needs to be saved.
        /// </summary>
        /// <param name="parent">The parent whose sort order should be ensured.</param>
        /// <returns>An enumeration of items whose sort order was updated and needs to be saved.</returns>
        public static IEnumerable<ContentItem> EnsureChildrenSortOrder(this ContentItem parent)
        {
            if (parent != null)
            {
                foreach (SortChildrenAttribute attribute in parent.GetContentType().GetCustomAttributes(typeof(SortChildrenAttribute), true))
                {
                    foreach (ContentItem updatedItem in attribute.ReorderChildren(parent))
                    {
                        yield return updatedItem;
                    }
                }
            }
        }
    }
}
