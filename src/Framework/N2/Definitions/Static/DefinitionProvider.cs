using System.Collections.Generic;
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

		/// <summary>The order this definition provider should be invoked, default 0.</summary>
		public int SortOrder { get { return -100; } }
	}
}
