using N2.Details;
using N2.Integrity;
using System;
using N2.Definitions;

namespace N2.Templates.Items
{
	/// <summary>
	/// A page item with a convenient set of properties defined by default.
	/// </summary>
	[WithEditableName("Name", 20, ContainerName = Tabs.Content),
		WithEditablePublishedRange("Published Between", 30, ContainerName = Tabs.Advanced, BetweenText = " and ")]
	[AvailableZone("Right", Zones.Right),
        AvailableZone("Recursive Right", Zones.RecursiveRight),
        AvailableZone("Left", Zones.Left),
        AvailableZone("Recursive Left", Zones.RecursiveLeft),
        AvailableZone("Content", Zones.Content),
        AvailableZone("Recursive Above", Zones.RecursiveAbove),
    	AvailableZone("Recursive Below", Zones.RecursiveBelow)]
	[RestrictParents(typeof(IStructuralPage))]
	public abstract class AbstractContentPage : AbstractPage
	{
		[EditableFreeTextArea("Text", 100, ContainerName = Tabs.Content)]
		public virtual string Text
		{
			get { return (string) (GetDetail("Text") ?? string.Empty); }
			set { SetDetail("Text", value, string.Empty); }
		}

		[EditableCheckBox("Visible", 40, ContainerName = Tabs.Advanced)]
		public override bool Visible
		{
			get { return base.Visible; }
			set { base.Visible = value; }
		}
	}
}