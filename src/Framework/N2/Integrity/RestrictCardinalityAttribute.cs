using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Definitions;

namespace N2.Integrity
{
    /// <summary>
    /// Restricts the number of siblings an item may have.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class RestrictCardinalityAttribute : AbstractDefinitionRefiner, IAllowedDefinitionFilter, IInheritableDefinitionRefiner
    {
        public RestrictCardinalityAttribute()
        {
            MaximumCount = 1;
        }

        public override void Refine(ItemDefinition currentDefinition, IList<ItemDefinition> allDefinitions)
        {
            currentDefinition.AllowedParentFilters.Add(this);
        }

        /// <summary>The maximum number of items of the attributed type allowed. Default is 1.</summary>
        public int MaximumCount { get; set; }

        /// <summary>The type of element that are considered for maximum cardinality. Default is the type decorated by this attribute.</summary>
        public Type ComparableType { get; set; }

        #region IAllowedDefinitionFilter Members

        public AllowedDefinitionResult IsAllowed(AllowedDefinitionQuery query)
        {
            if (query.ParentDefinition == null) return AllowedDefinitionResult.DontCare;
            if (query.Parent == null) return AllowedDefinitionResult.DontCare;

            var type = ComparableType ?? query.ChildDefinition.ItemType;
            int childrenOfTypeCount = query.Parent.Children.Count(i => (query.Child == null || query.Child.ID != i.ID) && type.IsAssignableFrom(i.GetContentType()));
            if (childrenOfTypeCount >= MaximumCount)
                return AllowedDefinitionResult.Deny;

            return AllowedDefinitionResult.DontCare;
        }

        #endregion
    }
}
