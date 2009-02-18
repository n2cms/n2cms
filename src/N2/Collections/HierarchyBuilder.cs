namespace N2.Collections
{
	/// <summary>
	/// Abstract base class for hierarchy builders.
	/// </summary>
	public abstract class HierarchyBuilder
	{
		private ItemFilter[] filters = null;

		/// <summary>
		/// Gets or sets the filters.
		/// </summary>
		/// <value>The filters.</value>
		public ItemFilter[] Filters
		{
			get { return filters; }
			set { filters = value; }
		}

		/// <summary>
		/// Builds the hierachy.
		/// </summary>
		/// <returns></returns>
		public abstract HierarchyNode<ContentItem> Build();

		/// <summary>
		/// Builds the hierachy using the specified filters.
		/// </summary>
		/// <param name="filters">The filters.</param>
		/// <returns></returns>
		public HierarchyNode<ContentItem> Build(params ItemFilter[] filters)
		{
			this.filters = filters;
			return Build();
		}

		/// <summary>
		/// Gets the children.
		/// </summary>
		/// <param name="currentItem">The current item.</param>
		/// <returns>A list of content items</returns>
		protected virtual ItemList GetChildren(ContentItem currentItem)
		{
			return Filters == null
				? currentItem.GetChildren()
				: currentItem.GetChildren(Filters);
		}
	}
}