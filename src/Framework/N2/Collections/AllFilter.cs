using System;
using System.Linq;
using System.Collections.Generic;

namespace N2.Collections
{
	/// <summary>
	/// Applies a collection of filters.
	/// </summary>
	[Obsolete("Use AllFilter. CompositeFilter will be removed in N2CMS 3.0.")]
	public class CompositeFilter : AllFilter
	{
		public CompositeFilter(params ItemFilter[] filters)
			: base(filters)
		{
		}

		public CompositeFilter(IEnumerable<ItemFilter> filters)
			: base(filters)
		{
		}
	}

	/// <summary>
	/// Applies a collection of filters.
	/// </summary>
	public class AllFilter : ItemFilter
	{
		private ItemFilter[] filters;

		public AllFilter(params ItemFilter[] filters)
		{
			this.filters = filters ?? new ItemFilter[0];
		}

		public AllFilter(IEnumerable<ItemFilter> filters)
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
			return filters.All(filter => filter.Match(item));
		}

		public static ItemFilter Wrap(IList<ItemFilter> filters)
		{
			if (filters == null || filters.Count == 0)
				return new NullFilter();
			if (filters.Count == 1)
				return filters[0];
			return new AllFilter(filters);
		}

		public static ItemFilter Wrap(params ItemFilter[] filters)
		{
			return Wrap((IList<ItemFilter>)filters);
		}

		public override string ToString()
		{
			return "(" + string.Join("&", filters.Select(f => f.ToString()).ToArray()) + ")";
		}
	}
}
