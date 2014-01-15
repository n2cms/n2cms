using System.Collections.Generic;

namespace N2.Collections
{
	/// <summary>
	/// Restricts a result set to only contain pages.
	/// </summary>
	public class PageFilter : ItemFilter
	{
		public PageFilter()
		{ }

		public PageFilter(bool requirePublished, bool requireAuthorized, bool requireVisible)
		{
			RequirePublished = requirePublished;
			RequireAuthorized = requireAuthorized;
			RequireVisible = requireVisible;
		}

		public bool RequirePublished { get; set; }
		public bool RequireAuthorized { get; set; }
		public bool RequireVisible { get; set; }

		/// <summary>Matches an item against the current filter.</summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public override bool Match(ContentItem item)
		{
			var isMatch = item.IsPage;
			if (isMatch && RequirePublished)
				isMatch &= item.IsPublished() && !item.IsExpired();
			if (isMatch && RequireVisible)
				isMatch &= item.Visible;
			if (isMatch && RequireAuthorized)
				isMatch &= new AccessFilter().Match(item);
			return isMatch;
		}

		/// <summary>Filters out items which aren't pages.</summary>
		/// <param name="items"></param>
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
