using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Definitions
{
	public class ReplaceDefinitionsAttribute : Attribute, IDefinitionRefiner
	{
		Type[] replacedDefinitions;

		public ReplaceDefinitionsAttribute(params Type[] replacedDefinitions)
		{
			this.replacedDefinitions = replacedDefinitions;
		}

		public void Refine(ItemDefinition currentDefinition, IList<ItemDefinition> allDefinitions)
		{
			foreach (ItemDefinition definition in allDefinitions)
			{
				foreach (Type t in replacedDefinitions)
				{
					if (definition.ItemType == t)
					{
						definition.Enabled = false;
					}
				}
			}
		}
	}
}
