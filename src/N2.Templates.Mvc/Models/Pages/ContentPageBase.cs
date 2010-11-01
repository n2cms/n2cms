using System;
using N2.Definitions;
using N2.Details;
using N2.Integrity;
using N2.Web.UI;

namespace N2.Templates.Mvc.Models.Pages
{
	/// <summary>
	/// A page item with a convenient set of properties defined by default.
	/// </summary>
	[WithEditableName("Name", 14, ContainerName = Tabs.Details),
	 WithEditablePublishedRange("Published Between", 16, ContainerName = Tabs.Details, BetweenText = " and ")]
	[AvailableZone("Right", Zones.Right), // declaring zones in this way allows managers to add parts from the management UI (not only drag-and-drop)
	 AvailableZone("Recursive Right", Zones.RecursiveRight),
	 AvailableZone("Left", Zones.Left),
	 AvailableZone("Recursive Left", Zones.RecursiveLeft),
	 AvailableZone("Content", Zones.Content),
	 AvailableZone("Recursive Above", Zones.RecursiveAbove),
	 AvailableZone("Recursive Below", Zones.RecursiveBelow)]
	[RestrictParents(typeof (IStructuralPage))]
	[Separator("TitleSeparator", 15, ContainerName = Tabs.Details)]
	public abstract class ContentPageBase : PageBase, ICommentable
	{
		// editables

		[EditableFreeTextArea("Text", 100, ContainerName = Tabs.Content)]
		public virtual string Text { get; set; }

		[EditableCheckBox("Visible", 12, ContainerName = Tabs.Details)]
		public override bool Visible
		{
			get { return base.Visible; }
			set { base.Visible = value; }
		}
	}

	[Obsolete("Use ContentPageBase and [PageDefinition]")]
	public abstract class AbstractContentPage : ContentPageBase
	{
	}
}