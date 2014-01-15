using N2.Details;
using N2.Integrity;
using N2.Definitions;

namespace N2.Templates.Items
{
    [PartDefinition("Subscribe", 
        Description = "An RSS feed subscription box. An RSS link is also added to the page enabling subscription through the browser's address field.",
        SortOrder = 370,
        IconUrl = "~/Templates/UI/Img/feed_link.png")]
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
