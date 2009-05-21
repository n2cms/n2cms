using System;
using System.Collections.Generic;

namespace N2.Definitions
{
	/// <summary>
	/// Replaces the parent item definition with the one decorated by this 
	/// attribute. This can be used to disable and replace items in external
	/// class libraries.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class ReplacesParentDefinitionAttribute : AbstractDefinitionRefiner, IDefinitionRefiner
	{
		DefinitionReplacementMode replacementMode = DefinitionReplacementMode.Remove;

		public DefinitionReplacementMode ReplacementMode
		{
			get { return replacementMode; }
			set { replacementMode = value; }
		}

		public override void Refine(ItemDefinition currentDefinition, IList<ItemDefinition> allDefinitions)
		{
			Type t = currentDefinition.ItemType;
			foreach (ItemDefinition definition in new List<ItemDefinition>(allDefinitions))
			{
				if (definition.ItemType == t.BaseType)
				{
					if(ReplacementMode == DefinitionReplacementMode.Remove)

					definition.Enabled = false;
					return;
				}
			}
		}
	}
}
