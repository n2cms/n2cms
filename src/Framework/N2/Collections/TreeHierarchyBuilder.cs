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
            HierarchyNode<ContentItem> parent = new HierarchyNode<ContentItem>(currentItem);
            if (remainingDepth > 1 && currentItem != null)
            {
                foreach (ContentItem childItem in GetChildren(currentItem))
                {
                    var childNode = BuildHierarchyTree(childItem, remainingDepth - 1);
                    childNode.Parent = parent;
                    parent.Children.Add(childNode);
                }
            }
            return parent;
        }
    }
}
