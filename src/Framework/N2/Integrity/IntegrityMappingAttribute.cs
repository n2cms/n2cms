using System;
using System.Collections.Generic;
using N2.Definitions;

namespace N2.Integrity
{
    /// <summary>
    /// Attribute used to map allowed children to an item type. This attribute 
    /// is useful in scenarios where you can't change the item definitions, 
    /// e.g. if they are defined in an external assembly.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class IntegrityMappingAttribute : TypeIntegrityAttribute, IDefinitionRefiner, IAllowedDefinitionFilter
    {
        private readonly Type parentType = typeof(ContentItem);
        private readonly IntegrityMappingOption option = IntegrityMappingOption.AddToExising;

        public IntegrityMappingAttribute(Type parentType)
        {
            this.parentType = parentType;
        }

        public IntegrityMappingAttribute(Type parentType, IntegrityMappingOption option, params Type[] allowedChildTypes)
            : this(parentType)
        {
            this.parentType = parentType;
            this.option = option;
            Types = allowedChildTypes;
        }

        public override void Refine(ItemDefinition currentDefinition, IList<ItemDefinition> allDefinitions)
        {
            foreach (var d in allDefinitions)
            {
                if (option == IntegrityMappingOption.AddToExising && !parentType.IsAssignableFrom(d.ItemType))
                    continue;

                d.AllowedChildFilters.Add(this);
            }
            //if (parentType.IsAssignableFrom(currentDefinition.ItemType))
            //{
            //    foreach (ItemDefinition definition in allDefinitions)
            //    {
            //        bool assignable = IsAssignable(definition.ItemType);
            //        if(assignable)
            //            currentDefinition.AddAllowedChild(definition);
            //        else if (option == IntegrityMappingOption.RemoveOthers)
            //            currentDefinition.RemoveAllowedChild(definition);
            //    }
            //}
        }

        #region IAllowedDefinitionFilter Members

        public AllowedDefinitionResult IsAllowed(AllowedDefinitionQuery context)
        {
            if (parentType.IsAssignableFrom(context.ParentDefinition.ItemType) && IsAssignable(context.ChildDefinition.ItemType))
                return AllowedDefinitionResult.Allow;
            else
                return (option == IntegrityMappingOption.AddToExising) ? AllowedDefinitionResult.DontCare : AllowedDefinitionResult.Deny;
        }

        #endregion
    }
}
