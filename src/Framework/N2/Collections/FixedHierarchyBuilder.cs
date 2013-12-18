using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Collections
{
    /// <summary>
    /// Builds a fixed hierarchy.
    /// </summary>
    public class FixedHierarchyBuilder : HierarchyBuilder
    {
        HierarchyNode<ContentItem> rootNode;


        public FixedHierarchyBuilder(HierarchyNode<ContentItem> rootNode)
        {
            this.rootNode = rootNode;
        }

        
        /// <summary>
        /// Builds the hierachy.
        /// </summary>
        /// <returns></returns>
        public override HierarchyNode<ContentItem> Build()
        {
            if (base.Filter != null)
                return rootNode.Clone(Filter);
            return rootNode;
        }
    }
}
