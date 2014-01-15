using System;
using System.Collections.Generic;

namespace N2.Definitions
{
    /// <summary>
    /// Base interface for refiners.
    /// </summary>
    public interface ISortableRefiner : IComparable<ISortableRefiner>
    {
        /// <summary>Can be used by the comparer to help with sorting.</summary>
        int RefinementOrder { get; }

        /// <summary>Alters the item definition.</summary>
        /// <param name="currentDefinition">The definition to alter.</param>
        /// <param name="allDefinitions">All definitions.</param>
        void Refine(ItemDefinition currentDefinition, IList<ItemDefinition> allDefinitions);
    }
}
