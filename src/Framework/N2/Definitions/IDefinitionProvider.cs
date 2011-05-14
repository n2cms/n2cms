using System.Collections.Generic;

namespace N2.Definitions
{
	public interface IDefinitionProvider
	{
		IEnumerable<ItemDefinition> GetDefinitions();
	}
}
