using N2;
using N2.Details;
using N2.Integrity;
using N2.Templates.Items;
using N2.Web.UI.WebControls;

namespace N2.Templates.Advertisement.Items
{
	[Definition("Social bookmarks")]
	[RestrictParents(typeof(Templates.Items.AbstractContentPage))]
	[AllowedZones(Zones.SiteRight, Zones.Right, Zones.RecursiveRight, Zones.SiteLeft)]
	[WithEditableTitle("Title", 90)]
	public class SocialBookmarks : AbstractItem
	{
		[Displayable(typeof(H4), "Text")]
		public override string Title
		{
			get { return base.Title; }
			set { base.Title = value; }
		}

		[EditableCheckBox("Show text", 100)]
		public virtual bool ShowText
		{
			get { return (bool)(GetDetail("ShowText") ?? true); }
			set { SetDetail("ShowText", value, true); }
		}

		public override string TemplateUrl
		{
			get { return "~/Advertisement/UI/SocialBookmarks.ascx"; }
		}
		public override bool IsPage
		{
			get { return false; }
		}

		public override string IconUrl
		{
			get { return "~/Advertisement/UI/Img/digg.png"; }
		}
	}
}
