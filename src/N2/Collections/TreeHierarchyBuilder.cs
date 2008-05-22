using System.Collections.Generic;

namespace N2.Collections
{
	/// <summary>
	/// Creates a hierarchy of content items from a certain level and downwards.
	/// </summary>
	public class TreeHierarchyBuilder : HierarchyBuilder
	{
		private readonly ContentItem rootItem;
		private readonly int maxDepth;

		public TreeHierarchyBuilder(ContentItem rootItem)
			: this(rootItem, int.MaxValue)
		{
		}

		public TreeHierarchyBuilder(ContentItem rootItem, int maxDepth)
		{
			this.rootItem = rootItem;
			this.maxDepth = maxDepth;
		}

		public override HierarchyNode<ContentItem> Build()
		{
			return BuildHierarchyTree(rootItem, maxDepth);
		}

		protected virtual HierarchyNode<ContentItem> BuildHierarchyTree(ContentItem currentItem, int remainingDepth)
		{
			HierarchyNode<ContentItem> node = new HierarchyNode<ContentItem>(currentItem);
			if (remainingDepth > 1)
			{
				foreach (ContentItem childItem in GetChildren(currentItem))
				{
					node.Children.Add(BuildHierarchyTree(childItem, remainingDepth - 1));
				}
			}
			return node;
		}
	}
}