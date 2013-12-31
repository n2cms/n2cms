using System.Collections.Generic;

namespace N2.Collections
{
    /// <summary>
    /// Filters items that arn't parts.
    /// </summary>
    public class PartFilter : ItemFilter
    {
        public override bool Match(ContentItem item)
        {
            return !item.IsPage;
        }

        public static void FilterParts(IList<ContentItem> items)
        {
            Filter(items, new PartFilter());
        }

        public override string ToString()
        {
            return "IsPart";
        }
    }
}
