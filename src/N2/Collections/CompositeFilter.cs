using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Collections
{
	/// <summary>
	/// Applies a collection of filters.
	/// </summary>
	public class CompositeFilter : ItemFilter
	{
        private ItemFilter[] filters;

        public CompositeFilter(params ItemFilter[] filters)
        {
            this.filters = filters ?? new ItemFilter[0];
        }

        public CompositeFilter(IEnumerable<ItemFilter> filters)
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
				if (!filter.Match(item))
					return false;
			return true;
		}
	}
}
