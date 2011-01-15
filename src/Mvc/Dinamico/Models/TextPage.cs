using N2;
using N2.Definitions;
using N2.Details;

namespace Dinamico.Models
{
	[WithEditableTitle, WithEditableName]
	public abstract class TextPage : ContentItem,
		IStructuralPage // this interface can be used by modules to interact with this app
	{
		[EditableFreeTextArea]
		public virtual string Text { get; set; }
	}
}