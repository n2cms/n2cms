using N2;
using N2.Details;
using N2.Integrity;
using N2.Templates.Items;
using N2.Web.UI.WebControls;

namespace N2.Templates.Items
{
	[PartDefinition("Social bookmarks",
		IconUrl = "~/Templates/UI/Img/digg.png")]
    [RestrictParents(typeof(AbstractContentPage))]
    [AllowedZones(Zones.SiteRight, Zones.Right, Zones.RecursiveRight, Zones.SiteLeft)]
    [WithEditableTitle("Title", 90)]
    public class SocialBookmarks : AbstractItem
    {
		[DisplayableHeading(4)]
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

        protected override string TemplateName
        {
            get { return "SocialBookmarks"; }
        }
    }
}