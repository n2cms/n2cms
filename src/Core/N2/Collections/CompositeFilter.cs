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
		private readonly ItemFilter[] filters;

		public CompositeFilter(params ItemFilter[] filters)
		{
			this.filters = filters;
		}

		public override bool Match(ContentItem item)
		{
			if (filters != null)
				foreach (ItemFilter filter in filters)
					if (!filter.Match(item))
						return false;
			return true;
		}
	}
}
