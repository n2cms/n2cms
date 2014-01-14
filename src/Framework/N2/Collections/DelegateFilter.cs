using System;

namespace N2.Collections
{
    /// <summary>
    /// Helps passing a delegate function used to determine wheter the item should be filtered.
    /// </summary>
    public class DelegateFilter : ItemFilter
    {
        readonly Func<ContentItem, bool> isPositiveMatch;

        public DelegateFilter(Func<ContentItem, bool> isPositiveMatch)
        {
            if (isPositiveMatch == null) throw new ArgumentNullException("isPositiveMatch");

            this.isPositiveMatch = isPositiveMatch;
        }

        public override bool Match(ContentItem item)
        {
            return isPositiveMatch(item);
        }

        public override string ToString()
        {
            return "Delegate";
        }
    }
}
