using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Collections
{
    /// <summary>
    /// Builds a hierarchy and moves the root node from the root position to first child position.
    /// </summary>
    public class ParallelRootHierarchyBuilder : HierarchyBuilder
    {
        HierarchyBuilder inner;

        public ParallelRootHierarchyBuilder(ContentItem rootItem)
            : this(rootItem, int.MaxValue)
        {
        }

        public ParallelRootHierarchyBuilder(ContentItem rootItem, int maxDepth)
        {
            inner = new TreeHierarchyBuilder(rootItem, maxDepth);
        }

        public override HierarchyNode<ContentItem> Build()
        {
            inner.GetChildren = GetChildren;
            var node = inner.Build();

            node.Children.Insert(0, new HierarchyNode<ContentItem>(node.Current) { Parent = node });
            node.Current = null;

            return node;
        }
    }
}
