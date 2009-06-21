using N2.Definitions;

namespace N2.Tests.Definitions.Items
{
	[PageDefinition]
	[RemoveDefinitions(DefinitionReplacementMode.Disable, typeof(DefinitionOne))]
	public class DefinitionReplacesNumber1 : N2.ContentItem
	{
	}
}