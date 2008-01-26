using N2.Details;
using N2.Integrity;

namespace N2.Templates.Items.Parts
{
	[Definition("Control Panel", "ControlPanel")]
	[AllowedZones("RecursiveRight", "Right", "SiteLeft", "SiteRight")]
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