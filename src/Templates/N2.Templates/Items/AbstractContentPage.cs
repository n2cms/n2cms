using N2.Details;
using N2.Integrity;
using N2.Web.UI;

namespace N2.Templates.Items
{
	/// <summary>
	/// A page item with a convenient set of properties defined by default.
	/// </summary>
	[WithEditableName("Name", 20, ContainerName = "content"),
		WithEditablePublishedRange("Published Between", 30, ContainerName = "advanced", BetweenText = " and ")]
	[TabPanel("advanced", "Advanced", 100)]
	[AvailableZone("Right", "Right"),
		AvailableZone("Left", "Left"),
		AvailableZone("Content", "Content"),
		AvailableZone("Recursive Right", "RecursiveRight"), 
		AvailableZone("Recursive Above", "RecursiveAbove")]
	[RestrictParents(typeof(IStructuralPage))]
	public abstract class AbstractContentPage : AbstractPage
	{
		[EditableCheckBox("Visible", 40, ContainerName = "advanced")]
		public override bool Visible
		{
			get{return base.Visible;}
			set{base.Visible = value;}
		}

		[EditableFreeTextArea("Text", 100, ContainerName = "content")]
		public virtual string Text
		{
			get { return (string) (GetDetail("Text") ?? string.Empty); }
			set { SetDetail("Text", value, string.Empty); }
		}
	}
}