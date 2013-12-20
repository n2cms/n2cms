using System.Collections.Generic;

namespace N2.Collections
{
    /// <summary>
    /// Filters items that arn't pages.
    /// </summary>
    public class PageFilter : ItemFilter
    {
        public override bool Match(ContentItem item)
        {
            return item.IsPage;
        }

        public static void FilterPages(IList<ContentItem> items)
        {
            Filter(items, new PageFilter());
        }

        public override string ToString()
        {
            return "IsPage";
        }
    }
}
