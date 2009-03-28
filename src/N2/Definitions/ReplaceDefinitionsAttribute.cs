using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Definitions
{
	/// <summary>
	/// Disables an item definition. Can be used to create a better 
	/// implementation of a definition in an existing solution. Note that this
	/// attribute doesn't modify any existing data. It only removes types from 
	/// the items that can be created.
	/// </summary>
	public class ReplaceDefinitionsAttribute : AbstractDefinitionRefiner, IDefinitionRefiner
	{
		Type[] replacedDefinitions;

		public ReplaceDefinitionsAttribute(params Type[] replacedDefinitions)
		{
			this.replacedDefinitions = replacedDefinitions;
		}

		public ReplaceDefinitionsAttribute(Type replacedDefinition)
		{
			this.replacedDefinitions = new Type[] { replacedDefinition };
		}

		public override void Refine(ItemDefinition currentDefinition, IList<ItemDefinition> allDefinitions)
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
