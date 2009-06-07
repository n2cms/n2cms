using N2.Details;
using N2.Integrity;
using N2.Templates.Mvc.Items.Pages;
using N2.Web.UI.WebControls;

namespace N2.Templates.Mvc.Items.Items
{
	[Definition("JQuery Social bookmarks")]
	[RestrictParents(typeof (AbstractContentPage))]
	[AllowedZones(Zones.SiteRight, Zones.Right, Zones.RecursiveRight, Zones.SiteLeft)]
	[WithEditableTitle("Title", 90)]
	public class JQuerySocialBookmarks : AbstractItem
	{
		[Displayable(typeof (H4), "Text")]
		public override string Title
		{
			get { return base.Title; }
			set { base.Title = value; }
		}

		[EditableCheckBox("Show text", 100)]
		public virtual bool ShowText
		{
			get { return (bool) (GetDetail("ShowText") ?? true); }
			set { SetDetail("ShowText", value, true); }
		}

		protected override string TemplateName
		{
			get { return "JQuerySocialBookmarks"; }
		}
	}
}