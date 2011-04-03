using N2;
using N2.Details;
using N2.Persistence;

namespace Dinamico.Models
{
	[PageDefinition]
	public class ContentPage : PageModelBase
	{
		[EditableFileUpload]
		[Persistable(Length = 256)] // to minimize select+1
		public virtual string Image { get; set; }

		[EditableText]
		[Persistable(Length = 1024)] // to minimize select+1
		public virtual string Summary { get; set; }

		[EditableFreeTextArea]
		public virtual string Text { get; set; }
	}
}