using System.Collections.Generic;

namespace N2.Definitions
{
	/// <summary>
	/// Provides definitions used for storing content in N2.
	/// </summary>
	public interface IDefinitionProvider
	{
		/// <summary>Gets definitions exposed by this proivder.</summary>
		/// <returns>An enumeration of definition available for creating new items. The result may be cached for future usage.</returns>
		IEnumerable<ItemDefinition> GetDefinitions();

		/// <summary>The order this definition provider should be invoked, default 0.</summary>
		int SortOrder { get; }
	}
}
