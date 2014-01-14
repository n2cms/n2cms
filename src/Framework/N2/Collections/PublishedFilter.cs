namespace N2.Collections
{
	/// <summary>
	/// Filters unpublished items.
	/// </summary>
	public class PublishedFilter : ItemFilter
	{
		/// <summary>Tells whether the item is published, i.e. now is between it's published and expires dates.</summary>
		/// <param name="item">The item to check.</param>
		/// <returns>True if the item is published</returns>
		public static bool IsPublished(ContentItem item)
		{
			return item.IsPublished() && !item.IsExpired();
		}

		public override bool Match(ContentItem item)
		{
			return IsPublished(item);
		}

		public override string ToString()
		{
			return "Published";
		}
	}
}
