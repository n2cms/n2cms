namespace N2.Collections
{
	/// <summary>
	/// Builds a hierarchy of nodes between a certain item and one of it's 
	/// ancestors (or the root item). This is useful in certain situations when
	/// creating a navigation menu.
	/// </summary>
	public class BranchHierarchyBuilder : HierarchyBuilder
	{
		private readonly ContentItem initialItem;
		private readonly ContentItem lastAncestor;

		public BranchHierarchyBuilder(ContentItem initialItem, ContentItem lastAncestor)
		{
			this.initialItem = initialItem;
			this.lastAncestor = lastAncestor;
		}

		public override HierarchyNode<ContentItem> Build()
		{
			if (initialItem == lastAncestor)
			{
				return new HierarchyNode<ContentItem>(initialItem);
			}

			HierarchyNode<ContentItem> previousNode = null;
			foreach (ContentItem currentItem in Find.EnumerateParents(initialItem, lastAncestor))
			{
				HierarchyNode<ContentItem> currentNode = new HierarchyNode<ContentItem>(currentItem);
				if (previousNode != null)
					previousNode.Parent = currentNode;
				foreach (ContentItem childItem in GetChildren(currentItem))
				{
					if (previousNode != null && previousNode.Current == childItem)
					{
						currentNode.Children.Add(previousNode);
					}
					else
					{
						HierarchyNode<ContentItem> childNode = new HierarchyNode<ContentItem>(childItem);
						currentNode.Children.Add(childNode);
						childNode.Parent = currentNode;
					}
				}
				previousNode = currentNode;
			}
			return previousNode;
		}
	}
}