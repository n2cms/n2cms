using System;

namespace N2.Collections
{
	/// <summary>
	/// Filters unpublished items.
	/// </summary>
	public class PublishedFilter : ItemFilter
	{
		public override bool Match(ContentItem item)
		{
			return (item.Published.HasValue && item.Published.Value <= DateTime.Now)
				&& !(item.Expires.HasValue && item.Expires.Value < DateTime.Now);
		}
	}
}
