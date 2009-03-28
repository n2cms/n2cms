using N2.Integrity;

namespace N2.Tests.Definitions.Items
{
	[Definition]
	[RestrictChildren(typeof(DefinitionOppressedChild))]
	public class DefinitionControllingParent : ContentItem
	{
	}
}