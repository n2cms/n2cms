namespace N2.Collections
{
	/// <summary>
	/// Abstract base class for hierarchy builders.
	/// </summary>
	public abstract class HierarchyBuilder
	{
		private ItemFilter[] filters = null;

		public ItemFilter[] Filters
		{
			get { return filters; }
			set { filters = value; }
		}

		public abstract HierarchyNode<ContentItem> Build();
		public HierarchyNode<ContentItem> Build(params ItemFilter[] filters)
		{
			this.filters = filters;
			return Build();
		}

		protected virtual ItemList GetChildren(ContentItem currentItem)
		{
			return Filters == null
				? currentItem.GetChildren()
				: currentItem.GetChildren(Filters);
		}
	}
}