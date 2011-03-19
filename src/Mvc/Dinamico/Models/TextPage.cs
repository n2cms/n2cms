using N2;
using N2.Definitions;
using N2.Details;
using N2.Web.UI;

namespace Dinamico.Models
{
	[WithEditableTitle, WithEditableName]
	[TabContainer(Constants.Containers.Content, "Content", 1000)]
	public abstract class TextPage : ContentItem,
		IStructuralPage // this interface can be used by modules to interact with this app
	{
		[EditableFreeTextArea]
		public virtual string Text { get; set; }
	}
}