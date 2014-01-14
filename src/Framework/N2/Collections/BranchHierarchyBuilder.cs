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
        bool appendAdditionalLevel = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="BranchHierarchyBuilder"/> class.
        /// </summary>
        /// <param name="initialItem">The initial item.</param>
        /// <param name="lastAncestor">The last ancestor.</param>
        /// <param name="appendAdditionalLevel">if set to <c>true</c> [append additional level].</param>
        public BranchHierarchyBuilder(ContentItem initialItem, ContentItem lastAncestor, bool appendAdditionalLevel)
            : this(initialItem, lastAncestor)
        {
            this.appendAdditionalLevel = appendAdditionalLevel;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BranchHierarchyBuilder"/> class.
        /// </summary>
        /// <param name="initialItem">The initial item.</param>
        /// <param name="lastAncestor">The last ancestor.</param>
        public BranchHierarchyBuilder(ContentItem initialItem, ContentItem lastAncestor)
        {
            this.initialItem = initialItem;
            this.lastAncestor = lastAncestor;
            UseMasterVersion = true;
        }

        /// <summary>
        /// Builds this instance.
        /// </summary>
        /// <returns></returns>
        public override HierarchyNode<ContentItem> Build()
        {
            if (initialItem == lastAncestor && !appendAdditionalLevel)
            {
                return new HierarchyNode<ContentItem>(initialItem);
            }

            HierarchyNode<ContentItem> previousNode = null;
            foreach (ContentItem currentItem in Find.EnumerateParents(initialItem, lastAncestor, appendAdditionalLevel, UseMasterVersion))
            {
                HierarchyNode<ContentItem> currentNode = new HierarchyNode<ContentItem>(currentItem);
                if (previousNode != null)
                {
                    previousNode.Parent = currentNode;
                }

                foreach (ContentItem childItem in GetChildren(currentItem))
                {
                    if (previousNode != null && childItem.Equals(previousNode.Current))
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

        public bool UseMasterVersion { get; set; }
    }
}
