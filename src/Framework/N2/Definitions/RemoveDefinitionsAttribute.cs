using System;
using System.Collections.Generic;

namespace N2.Definitions
{
	/// <summary>
	/// Disables an item definition. Can be used to create a better 
	/// implementation of a definition in an existing solution.
	/// </summary>
	/// <remarks>
	/// This attribute now may remove definitions or just disable them 
	/// depending on the ReplacementMode property.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
	public class RemoveDefinitionsAttribute : AbstractDefinitionRefiner, IDefinitionRefiner
	{
		Type[] replacedDefinitions;
		DefinitionReplacementMode replacementMode = DefinitionReplacementMode.Remove;



		public RemoveDefinitionsAttribute(DefinitionReplacementMode replacementMode, params Type[] replacedDefinitions)
			:this(replacedDefinitions)
		{
			ReplacementMode = replacementMode;
			this.replacedDefinitions = replacedDefinitions;
		}

		public RemoveDefinitionsAttribute(params Type[] replacedDefinitions)
		{
			this.replacedDefinitions = replacedDefinitions;
		}

		public RemoveDefinitionsAttribute(Type replacedDefinition)
		{
			replacedDefinitions = new[] { replacedDefinition };
		}


		/// <summary>
		/// Instructs the definition builder what to do with the removed 
		/// definitions.
		/// </summary>
		public DefinitionReplacementMode ReplacementMode
		{
			get { return replacementMode; }
			set { replacementMode = value; }
		}



		public override void Refine(ItemDefinition currentDefinition, IList<ItemDefinition> allDefinitions)
		{
			List<ItemDefinition> removedDefinitions = new List<ItemDefinition>();

			foreach (ItemDefinition definition in allDefinitions)
			{
				foreach (Type t in replacedDefinitions)
				{
					if(t.IsAssignableFrom(definition.ItemType))
					{
						removedDefinitions.Add(definition);
					}
				}
			}

			foreach(ItemDefinition definition in removedDefinitions)
			{
				if(ReplacementMode == DefinitionReplacementMode.Remove)
					allDefinitions.Remove(definition);
				else
					definition.Enabled = false;
			}
		}
	}
}
