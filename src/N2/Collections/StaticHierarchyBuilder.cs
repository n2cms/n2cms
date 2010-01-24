using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Collections
{
	/// <summary>Passes a hierarchy.</summary>
	public class StaticHierarchyBuilder : HierarchyBuilder
	{
		HierarchyNode<ContentItem> nodes;

		public StaticHierarchyBuilder(HierarchyNode<ContentItem> nodes)
		{
			this.nodes = nodes;
		}

		public override HierarchyNode<ContentItem> Build()
		{
			return nodes;
		}
	}
}
