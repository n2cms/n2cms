using N2;
using N2.Definitions;
using N2.Details;
using N2.Web.UI;

namespace Dinamico.Models
{
	[WithEditableTitle]
	[WithEditableName(ContainerName = "Metadata")]
	[WithEditableVisibility(ContainerName = "Metadata")]
	[SidebarContainer("Metadata", 100, HeadingText = "Metadata")]
	[TabContainer(Detaults.Containers.Content, "Content", 1000)]
	public abstract class TextPage : ContentItem,
		IStructuralPage // this interface can be used by modules to interact with this app
	{
		[EditableFreeTextArea]
		public virtual string Text { get; set; }
	}
}