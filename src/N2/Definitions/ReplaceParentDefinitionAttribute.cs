using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Definitions
{
	/// <summary>
	/// Replaces the parent item definition with the one decorated by this 
	/// attribute. This can be used to disable and replace items in external
	/// class libraries.
	/// </summary>
	public class ReplacesParentDefinitionAttribute : AbstractDefinitionRefiner, IDefinitionRefiner
	{
		public override void Refine(ItemDefinition currentDefinition, IList<ItemDefinition> allDefinitions)
		{
			Type t = currentDefinition.ItemType;
			foreach (ItemDefinition definition in allDefinitions)
			{
				if (definition.ItemType == t.BaseType)
				{
					definition.Enabled = false;
					return;
				}
			}
		}
	}
}
