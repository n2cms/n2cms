using N2.Definitions;
using N2.Details;

[assembly:RemoveEditable("Text", typeof(N2.Tests.Definitions.Items.DefinitionRemovable))]

namespace N2.Tests.Definitions.Items
{
	[Definition]
	public class DefinitionRemovable : ContentItem
	{
		[EditableTextBox]
		public string Description { get; set; }

		[EditableFreeTextArea]
		public string Text { get; set; }
	}
}