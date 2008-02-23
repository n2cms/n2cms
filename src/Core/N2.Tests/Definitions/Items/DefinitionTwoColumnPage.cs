using N2.Details;
using N2.Integrity;
using N2.Web.UI;

namespace N2.Tests.Definitions.Items
{
	[Definition]
	[AvailableZone("Right", "Right")]
	[WithEditableTitle("Title", 0, ContainerName = "general")]
	[WithEditableName("Name", 1)]
	[FieldSet("general", "General", 10)]
	public abstract class DefinitionTwoColumnPage : ContentItem
	{
	}
}