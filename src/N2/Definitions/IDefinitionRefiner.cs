using System;
using System.Collections.Generic;

namespace N2.Definitions
{
	/// <summary>
	/// Attributes implementing this interface can alter item definitions while
	/// they are beeing initiated.
	/// </summary>
	public interface IDefinitionRefiner
	{
		/// <summary>Alters the item definition.</summary>
		/// <param name="currentDefinition">The definition to alter.</param>
		/// <param name="allDefinitions">All definitions.</param>
		void Refine(ItemDefinition currentDefinition, IList<ItemDefinition> allDefinitions);
	}
}
