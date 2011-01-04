using N2.Details;
using N2.Integrity;
using N2.Web.UI;

namespace N2.Tests.Definitions.Items
{
	[PageDefinition]
	[AvailableZone("Right", "Right")]
	[WithEditableTitle("Title", 0, ContainerName = "general")]
	[WithEditableName("Name", 1)]
	[FieldSetContainer("general", "General", 10)]
	public abstract class DefinitionTwoColumnPage : ContentItem
	{
	}
}