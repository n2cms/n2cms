using N2.Integrity;

namespace N2.Tests.Definitions.Items
{
	[PartDefinition]
	[AllowedZones("Right")]
	public abstract class DefinitionRightColumnPart : N2.ContentItem
	{
		public override bool IsPage
		{
			get { return false; }
		}
	}
}
