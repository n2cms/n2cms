namespace N2.Collections
{
	/// <summary>
	/// Filters items not suitable for navigation. Basically it's a composition
	/// of page, visible, published, and access filter.
	/// </summary>
	public class NavigationFilter : CompositeFilter
	{
		public NavigationFilter()
			: base(new PageFilter(), new VisibleFilter(), new PublishedFilter(), new AccessFilter())
		{
		}
	}
}