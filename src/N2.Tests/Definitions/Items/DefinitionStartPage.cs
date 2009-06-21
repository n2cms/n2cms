using N2.Integrity;

namespace N2.Tests.Definitions.Items
{
	[PageDefinition]
	[AvailableZone("Left and Center", "LeftAndCenter")]
	[RestrictParents(AllowedTypes.None)]
	public class DefinitionStartPage : DefinitionTwoColumnPage, ILeftColumnlPage
	{
	}
}