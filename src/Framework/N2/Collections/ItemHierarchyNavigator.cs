using System;
using System.Collections.Generic;

namespace N2.Collections
{
    /// <summary>
    /// Navigates a graph of content item nodes.
    /// </summary>
    [Obsolete("Use hierarchy builders and hierarchy nodes")]
    public class ItemHierarchyNavigator : IHierarchyNavigator<ContentItem>
    {
        private readonly HierarchyNode<ContentItem> currentNode = null;

        public ItemHierarchyNavigator(HierarchyNode<ContentItem> currentNode)
        {
            this.currentNode = currentNode;
        }

        public ItemHierarchyNavigator(HierarchyBuilder builder, params ItemFilter[] filters)
        {
            currentNode = builder.Children(filters).Build();
        }

        public ItemHierarchyNavigator(HierarchyBuilder builder)
        {
            currentNode = builder.Build();
        }

        public HierarchyNode<ContentItem> CurrentNode
        {
            get { return currentNode; }
        }

        public ItemHierarchyNavigator GetRootHierarchy()
        {
            return new ItemHierarchyNavigator(GetRootNode());
        }

        public HierarchyNode<ContentItem> GetRootNode()
        {
            HierarchyNode<ContentItem> last = currentNode;
            while (last.Parent != null)
                last = last.Parent;
            return last;
        }

        public IEnumerable<ContentItem> EnumerateAllItems()
        {
            HierarchyNode<ContentItem> rootNode = GetRootNode();
            return EnumerateItemsRecursive(rootNode);
        }

        public IEnumerable<ContentItem> EnumerateChildItems()
        {
            return EnumerateItemsRecursive(currentNode);
        }

        protected virtual IEnumerable<ContentItem> EnumerateItemsRecursive(HierarchyNode<ContentItem> node)
        {
            yield return node.Current;
            foreach (HierarchyNode<ContentItem> childNode in node.Children)
            {
                foreach (ContentItem childItem in EnumerateItemsRecursive(childNode))
                {
                    yield return childItem;
                }
            }
        }

        #region IHierarchyItem<ContentItem> Members

        public IHierarchyNavigator<ContentItem> Parent
        {
            get
            {
                if (currentNode.Parent != null)
                    return new ItemHierarchyNavigator(currentNode.Parent);
                else
                    return null;
            }
        }

        public IEnumerable<IHierarchyNavigator<ContentItem>> Children
        {
            get
            {
                foreach (HierarchyNode<ContentItem> childNode in currentNode.Children)
                {
                    yield return new ItemHierarchyNavigator(childNode);
                }
            }
        }

        public ContentItem Current
        {
            get { return currentNode.Current; }
        }

        public bool HasChildren
        {
            get { return currentNode.Children.Count > 0; }
        }

        #endregion
    }
}
