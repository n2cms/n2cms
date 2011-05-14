using System;

namespace N2.Collections
{
	/// <summary>
	/// Abstract base class for hierarchy builders.
	/// </summary>
	public abstract class HierarchyBuilder
	{
		ChildFactoryDelegate getChildren = (item) => item.GetChildren();

		public ChildFactoryDelegate GetChildren
		{
			get { return getChildren; }
			set { getChildren = value; }
		}

		/// <summary>
		/// Gets or sets the filters.
		/// </summary>
		/// <value>The filters.</value>
		[Obsolete("Use GetChilren delegate instead", true)]
		public ItemFilter[] Filters
		{
			get { throw new NotSupportedException("Getting filters is no longer supported"); }
			set { Children(value); }
		}

		/// <summary>
		/// Builds the hierachy.
		/// </summary>
		/// <returns></returns>
		public abstract HierarchyNode<ContentItem> Build();

		/// <summary>Builds the hierachy using the specified filters.</summary>
		/// <param name="filters">The filters.</param>
		/// <returns></returns>
		[Obsolete("Use builder.Children(ChildFactoryDelegate).Build()", true)]
		public HierarchyNode<ContentItem> Build(params ItemFilter[] filters)
		{
			return Children(filters).Build();
		}

		/// <summary>Builds the hierachy using the specified child factory method.</summary>
		/// <param name="filters">The filters.</param>
		/// <returns></returns>
		public HierarchyBuilder Children(params ItemFilter[] filters)
		{
			GetChildren = (item) => item.GetChildren(filters);
			return this;
		}

		/// <summary>Builds the hierachy using the specified child factory method.</summary>
		/// <param name="filters">The filters.</param>
		/// <returns></returns>
		public HierarchyBuilder Children(ChildFactoryDelegate childFactory)
		{
			GetChildren = childFactory;
			return this;
		}
	}
}