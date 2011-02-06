using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Engine;

namespace N2.Definitions.Static
{
	[Service(typeof(IDefinitionProvider))]
	public class DefinitionProvider : IDefinitionProvider
	{
		ItemDefinition[] definitions;

		public DefinitionProvider(DefinitionBuilder definitionBuilder)
		{
			this.definitions = definitionBuilder.GetDefinitions().ToArray();
		}

		public IEnumerable<ItemDefinition> GetDefinitions()
		{
			return definitions;
		}
	}
}
