using N2.Integrity;
using N2.Templates.Items;

namespace N2.Templates.UI.Items.LayoutParts
{
	[Definition("Vertical menu", "VerticalMenu")]
	[RestrictParents(typeof(StartPage))] // The menu is placed on the start page and displayed on all underlying pages
	[AllowedZones("SiteLeft")]
	public class VerticalMenu : SidebarItem
	{
		[N2.Details.EditableTextBox("Starting depth", 100)]
		public virtual int StartingDepth
		{
			get { return (int)(GetDetail("StartingDepth") ?? 2); }
			set { SetDetail("StartingDepth", value, 1); }
		}

		public override string TemplateUrl
		{
			get { return "~/Layouts/Parts/VerticalMenu.ascx"; }
		}

		public override string IconUrl
		{
			get
			{
				return "~/Img/page_white_link.png";
			}
		}
	}
}
