using N2.Details;
using N2.Integrity;
using N2.Templates.Mvc.Items.Pages;

namespace N2.Templates.Mvc.Items.Items
{
	[PartDefinition("Subscribe",
		Description =
			"An RSS feed subscription box. An RSS link is also added to the page enabling subscription through the browser's address field."
		,
		SortOrder = 370,
		IconUrl = "~/Content/Img/feed_link.png")]
	[RestrictParents(typeof (IStructuralPage))]
	[AllowedZones(Zones.Right, Zones.Left, Zones.RecursiveRight, Zones.RecursiveLeft, Zones.SiteRight, Zones.SiteLeft)]
	public class Subscribe : SidebarItem
	{
		[EditableLink("Feed", 50)]
		public virtual RssFeed SelectedFeed
		{
			get { return GetDetail("SelectedFeed") as RssFeed; }
			set { SetDetail("SelectedFeed", value); }
		}

		protected override string TemplateName
		{
			get { return "Subscribe"; }
		}
	}
}