using System.Web.UI.WebControls;
using N2.Details;
using N2.Integrity;
using N2.Templates.Items;

namespace N2.Templates.Rss.Items
{
	[Definition("Subscribe", "Subscribe", "An RSS feed subscription box. An RSS link is also added to the page enabling subscription through the browser's address field.", "", 370)]
	[RestrictParents(typeof (IStructuralPage))]
	[AllowedZones("Right", "RecursiveRight", "SiteWideRight")]
	public class Subscribe : SidebarItem
	{
		[EditableLink("Feed", 50)]
		public virtual RssFeed SelectedFeed
		{
			get { return GetDetail("SelectedFeed") as RssFeed; }
			set { SetDetail("SelectedFeed", value); }
		}

		public override string IconUrl
		{
			get { return "~/Rss/UI/Img/feed_link.png"; }
		}

		public override bool IsPage
		{
			get { return false; }
		}

		public override string TemplateUrl
		{
			get { return "~/RSS/UI/Subscribe.ascx"; }
		}
	}
}