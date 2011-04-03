using N2.Definitions;
using N2.Details;

[assembly:RemoveEditable("Text", typeof(N2.Tests.Definitions.Items.DefinitionRemovable))]

namespace N2.Tests.Definitions.Items
{
	[PageDefinition]
	public class DefinitionRemovable : ContentItem
	{
		[EditableText]
		public string Description { get; set; }

		[EditableFreeTextArea]
		public string Text { get; set; }
	}
}