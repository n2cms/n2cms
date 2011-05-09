using N2.Integrity;

namespace N2.Tests.Definitions.Items
{
	[PartDefinition]
	[AllowedZones("Right", "LeftAndCenter")]
	[RestrictParents(typeof(DefinitionTwoColumnPage))]
	public class DefinitionTextItem : DefinitionRightColumnPart
	{
	}
}
