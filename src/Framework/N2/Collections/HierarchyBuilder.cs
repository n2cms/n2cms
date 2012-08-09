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


		protected ItemFilter Filter { get; set; }


		/// <summary>
		/// Builds the hierachy.
		/// </summary>
		/// <returns></returns>
		public abstract HierarchyNode<ContentItem> Build();

		/// <summary>Builds the hierachy using the specified child factory method.</summary>
		/// <param name="filters">The filters.</param>
		/// <returns></returns>
		public HierarchyBuilder Children(params ItemFilter[] filters)
		{
			Filter = (filters.Length == 1)
				? filters[0]
				: new AllFilter(filters);

			GetChildren = (item) => item.GetChildren(filters);
			return this;
		}

		/// <summary>Builds the hierachy using the specified child factory method.</summary>
		/// <param name="childFactory">The method resolving children.</param>
		/// <returns></returns>
		public HierarchyBuilder Children(ChildFactoryDelegate childFactory)
		{
			GetChildren = childFactory;
			return this;
		}
	}
}