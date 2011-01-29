using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Engine;
using N2.Definitions.Static;

namespace N2.Definitions
{
	[Service(typeof(IDefinitionProvider))]
	public class ReflectingDefinitionProvider : IDefinitionProvider
	{
		ItemDefinition[] definitions;

		public ReflectingDefinitionProvider(DefinitionBuilder definitionBuilder)
		{
			this.definitions = definitionBuilder.GetDefinitions().ToArray();
		}

		public IEnumerable<ItemDefinition> GetDefinitions()
		{
			return definitions;
		}
	}
}
