using N2.Details;
using N2.Integrity;
using N2.Templates.Items;

namespace N2.Templates.UI.Items.Parts
{
	[Definition("Control Panel", "ControlPanel")]
	[AllowedZones(Zones.RecursiveRight, Zones.Right, Zones.SiteLeft, Zones.SiteRight)]
	[WithEditableTitle("Title", 10)]
	public class ControlPanel : SidebarItem
	{
		public override string TemplateUrl
		{
			get { return "~/Parts/ControlPanel.ascx"; }
		}

		public override string IconUrl
		{
			get { return "~/Img/controller.png"; }
		}
	}
}