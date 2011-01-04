using N2.Integrity;

namespace N2.Tests.Definitions.Items
{
	[PageDefinition]
	[RestrictChildren(typeof(DefinitionOppressedChild))]
	public class DefinitionControllingParent : ContentItem
	{
	}
}