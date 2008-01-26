using System;
using System.Collections.Generic;

namespace N2.Definitions
{
	/// <summary>
	/// Attributes implementing this interface can alter item definitions while
	/// they are beeing initiated. All classes in the inheritance chain are 
	/// queried for this interface when refining the definition.
	/// </summary>
	public interface IInheritableDefinitionRefiner
	{
		/// <summary>Alters the item definition.</summary>
		/// <param name="currentDefinition">The definition to alter.</param>
		/// <param name="allDefinitions">All definitions.</param>
		void Refine(ItemDefinition currentDefinition, IList<ItemDefinition> allDefinitions);
	}
}
