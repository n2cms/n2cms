using System;
using System.Collections.Generic;

namespace N2.Definitions
{
    /// <summary>
    /// Base class providing sorting capabilities to refiner attributes.
    /// </summary>
    public abstract class AbstractDefinitionRefiner : Attribute, ISortableRefiner
    {
        public AbstractDefinitionRefiner()
        {
            RefinementOrder = Definitions.RefineOrder.Middle;
        }

        #region ISortableRefiner Members

        public int RefinementOrder { get; set; }

        public abstract void Refine(ItemDefinition currentDefinition, IList<ItemDefinition> allDefinitions);

        #endregion

        #region IComparable<ISortableRefiner> Members

        int IComparable<ISortableRefiner>.CompareTo(ISortableRefiner other)
        {
            return RefinementOrder - other.RefinementOrder;
        }

        #endregion
    }
}
