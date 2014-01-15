using System;
using System.Collections.Generic;
using System.Linq;
using N2.Definitions;

namespace N2.Integrity
{
    /// <summary>
    /// This attribute replace the children allowed with the types 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class RestrictChildrenAttribute : TypeIntegrityAttribute, IInheritableDefinitionRefiner, IAllowedDefinitionFilter
    {
        /// <summary>
        /// Restrict children by template name, allow only children with these template name.
        /// </summary>
        public string[] TemplateNames { get; set; }

        /// <summary>Initializes a new instance of the RestrictChildrenAttribute which is used to restrict which types of items may be added below which.</summary>
        public RestrictChildrenAttribute()
        {
            RefinementOrder = RefineOrder.Before + 10;
        }

        /// <summary>Initializes a new instance of the RestrictChildrenAttribute which is used to restrict which types of items may be added below which.</summary>
        /// <param name="allowedTypes">Defines wether all types of items are allowed as parent items.</param>
        public RestrictChildrenAttribute(AllowedTypes allowedTypes)
            : this()
        {
            if (allowedTypes == AllowedTypes.All)
                Types = null;
            else
                Types = new Type[0];
        }

        /// <summary>Initializes a new instance of the RestrictChildrenAttribute which is used to restrict which types of items may be added below which.</summary>
        /// <param name="allowedChildrenTypes">A list of allowed types. Null is interpreted as all types are allowed.</param>
        public RestrictChildrenAttribute(params Type[] allowedChildrenTypes)
            : this()
        {
            Types = allowedChildrenTypes;
        }

        public override void Refine(ItemDefinition currentDefinition, IList<ItemDefinition> allDefinitions)
        {
            currentDefinition.AllowedChildFilters.Add(this);
        }

        public IEnumerable<ItemDefinition> Filter(IDefinitionManager definitions, ItemDefinition currentDefinition, IEnumerable<ItemDefinition> allDefinitions)
        {
            return allDefinitions.Where(d => IsAssignable(d.ItemType));
        }

        #region IAllowedDefinitionFilter Members

        public AllowedDefinitionResult IsAllowed(AllowedDefinitionQuery context)
        {
            if (IsAssignable(context.ChildDefinition.ItemType))
                if(TemplateNames == null || TemplateNames.Contains(context.ChildDefinition.TemplateKey))
                    return AllowedDefinitionResult.DontCare;
            
            return AllowedDefinitionResult.Deny;
        }

        #endregion
    }
}
