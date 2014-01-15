using System;
using System.Collections.Generic;

namespace N2.Definitions
{
    /// <summary>
    /// Removes details from a content item. This can be used to modify editables on 
    /// external components whose code you don't control.
    /// </summary>
    /// <example>
    /// // the attribute can be added to an assembly...
    /// [assembly:N2.Definitions.RemoveEditable("Title", typeof(SomeItemInLibraryCode))]
    /// 
    /// // or to the content item inheriting an external item
    /// [RemoveEditable("Title")]
    /// public class MyItem : ExternalItem
    /// {
    /// }
    /// </example>
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class RemoveEditableAttribute : AbstractDefinitionRefiner, IDefinitionRefiner
    {
        public RemoveEditableAttribute(string editableOrContainerName)
        {
            Name = editableOrContainerName;
        }

        public RemoveEditableAttribute(string editableOrContainerName, Type affectedType)
        {
            Name = editableOrContainerName;
            AffectedType = affectedType;
        }

        /// <summary>The name of the detail to remove.</summary>
        public string Name { get; set; }
        /// <summary>The type of item to remove details from.</summary>
        public Type AffectedType { get; set; }

        public override void Refine(ItemDefinition currentDefinition, IList<ItemDefinition> allDefinitions)
        {
            if (AffectedType != null && !AffectedType.IsAssignableFrom(currentDefinition.ItemType))
                return;

            var containable = currentDefinition.GetNamed(Name);
            currentDefinition.RemoveRange(containable);
        }
    }
}
