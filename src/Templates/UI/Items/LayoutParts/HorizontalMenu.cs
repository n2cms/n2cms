using N2.Details;
using N2.Integrity;

namespace N2.Templates.Items.LayoutParts
{
	[Definition("Horizontal Menu", "HorizontalMenu")]
	[RestrictParents(typeof(StartPage))] // The menu is placed on the start page and displayed on all underlying pages
	[AllowedZones("SiteTop")]
	public class HorizontalMenu : AbstractItem
	{
		[EditableTextBox("Max depth", 100)]
		public virtual int MaxDepth
		{
			get { return (int)(GetDetail("MaxDepth") ?? 2); }
			set { SetDetail("MaxDepth", value, 2); }
		}

		[EditableCheckBox("Enable expanding menu", 110)]
		public virtual bool ExpandingMenu
		{
			get { return (bool)(GetDetail("ExpandingMenu") ?? false); }
			set { SetDetail("ExpandingMenu", value, false); }
		}

		public override string TemplateUrl
		{
			get { return "~/Layouts/Parts/HorizontalMenu.ascx"; }
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
