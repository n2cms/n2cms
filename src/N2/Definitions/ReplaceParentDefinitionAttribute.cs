using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Definitions
{
	public class ReplacesParentDefinitionAttribute : Attribute, IDefinitionRefiner
	{
		public void Refine(ItemDefinition currentDefinition, IList<ItemDefinition> allDefinitions)
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
