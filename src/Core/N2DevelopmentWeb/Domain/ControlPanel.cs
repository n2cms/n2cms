using N2.Details;
using N2.Web.UI.WebControls;
using N2;

namespace N2DevelopmentWeb.Domain
{
	[Definition("Control Panel", "ControlPanel")]
	[N2.Integrity.AllowedZones("Right")]
	[WithEditable("Zone", typeof(ZoneSelector), "SelectedValue", 50, "ZoneName")]
	[WithEditableTitle("Title", 10)]
	public class ControlPanel : N2.ContentItem
	{
		public override string TemplateUrl
		{
			get
			{
				return "~/Uc/ControlPanel.ascx";
			}
		}

		public override bool IsPage
		{
			get { return false; }
		}
	}
}
