using System.Collections.Generic;
using System;

namespace N2.Definitions
{
    /// <summary>
    /// Disables a definition removing it from lists when choosing new items. 
    /// Existing items will not be affaceted.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class DisableAttribute : AbstractDefinitionRefiner, IDefinitionRefiner
    {
        public override void Refine(ItemDefinition currentDefinition, IList<ItemDefinition> allDefinitions)
        {
            currentDefinition.Enabled = false;
        }
    }
}
