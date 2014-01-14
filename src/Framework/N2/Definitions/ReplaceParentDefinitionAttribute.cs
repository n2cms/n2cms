using System;
using System.Collections.Generic;

namespace N2.Definitions
{
    /// <summary>
    /// Replaces the parent item definition with the one decorated by this 
    /// attribute. This can be used to disable and replace items in external
    /// class libraries.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ReplacesParentDefinitionAttribute : AbstractDefinitionRefiner, IDefinitionRefiner
    {
        DefinitionReplacementMode replacementMode = DefinitionReplacementMode.Remove;
        bool assumeParentDiscriminator = true;

        /// <summary>
        /// Replacing the parent without removing it will result in a disabled
        /// definition. Existing items can be loaded but new ones can't be created
        /// through the editor UI.
        /// </summary>
        public DefinitionReplacementMode ReplacementMode
        {
            get { return replacementMode; }
            set { replacementMode = value; }
        }

        /// <summary>
        /// Set to false to assume the parent item's discriminator. This will cause the 
        /// decorated type to be loaded instead of the removed.
        /// </summary>
        public bool AssumeParentDiscriminator
        {
            get { return assumeParentDiscriminator; }
            set { assumeParentDiscriminator = value; }
        }

        public override void Refine(ItemDefinition currentDefinition, IList<ItemDefinition> allDefinitions)
        {
            Type t = currentDefinition.ItemType;
            foreach (ItemDefinition definition in new List<ItemDefinition>(allDefinitions))
            {
                if (definition.ItemType == t.BaseType)
                {
                    if (ReplacementMode == DefinitionReplacementMode.Remove)
                    {
                        allDefinitions.Remove(definition);
                        var shadowDefinition = currentDefinition.Clone();
                        shadowDefinition.Discriminator += "Disabled";
                        N2.Definitions.Static.DefinitionMap.Instance.SetDefinition(definition.ItemType, definition.TemplateKey, shadowDefinition);

                        if (AssumeParentDiscriminator)
                            currentDefinition.Discriminator = definition.Discriminator;
                    }
                    else
                        definition.Enabled = false;
                    return;
                }
            }
        }
    }
}
