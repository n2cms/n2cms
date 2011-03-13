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
		DefinitionBuilder definitionBuilder;

		public DefinitionProvider(DefinitionBuilder definitionBuilder)
		{
			this.definitionBuilder = definitionBuilder;
		}

		public IEnumerable<ItemDefinition> GetDefinitions()
		{
			return definitionBuilder.GetDefinitions();
		}
	}
}
