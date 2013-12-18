using System.Linq;
using System.Collections.Generic;

namespace N2.Collections
{
    /// <summary>
    /// Filters items not matching any of the filters.
    /// </summary>
    public class AnyFilter : ItemFilter
    {
        private ItemFilter[] filters;

        public AnyFilter(params ItemFilter[] filters)
        {
            this.filters = filters ?? new ItemFilter[0];
        }

        public AnyFilter(IEnumerable<ItemFilter> filters)
        {
            this.filters = new List<ItemFilter>(filters).ToArray();
        }

        /// <summary>
        /// The filters that compose this filter.
        /// </summary>
        public ItemFilter[] Filters
        {
            get { return filters; }
            set { filters = value; }
        }

        public override bool Match(ContentItem item)
        {
            foreach (ItemFilter filter in filters)
                if (filter.Match(item))
                    return true;
            return false;
        }

        public static ItemFilter Wrap(IList<ItemFilter> filters)
        {
            if (filters == null || filters.Count == 0)
                return new NullFilter();
            else if (filters.Count == 1)
                return filters[0];
            else
                return new AnyFilter(filters);
        }

        public static ItemFilter Wrap(params ItemFilter[] filters)
        {
            return Wrap((IList<ItemFilter>)filters);
        }

        public override string ToString()
        {
            return "(" + string.Join("|", filters.Select(f => f.ToString()).ToArray()) + ")";
        }
    }
}
