using N2;
using N2.Details;
using N2.Persistence;
using N2.Definitions;

namespace Dinamico.Models
{
	[PageDefinition]
	[WithEditableTemplateSelection(ContainerName = Defaults.Containers.Metadata)]
	public class ContentPage : PageModelBase
	{
		[EditableMediaUpload]
		[Persistable(Length = 256)] // to minimize select+1
		public virtual string Image { get; set; }

		[EditableText]
		[Persistable(Length = 1024)] // to minimize select+1
		public virtual string Summary { get; set; }

		[EditableFreeTextArea]
		[DisplayableTokens]
		public virtual string Text { get; set; }
	}
}