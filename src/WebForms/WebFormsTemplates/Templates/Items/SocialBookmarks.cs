using N2;
using N2.Details;
using N2.Integrity;
using N2.Templates.Items;
using N2.Web.UI.WebControls;
using N2.Definitions;
using N2.Web.UI;

namespace N2.Templates.Items
{
    [PartDefinition("Social bookmarks",
        IconUrl = "~/Templates/UI/Img/digg.png")]
    [RestrictParents(typeof(IPage))]
    [AllowedZones(Zones.SiteRight, Zones.Right, Zones.RecursiveRight, Zones.SiteLeft)]
    [RestrictCardinality]
    [FieldSetContainer("SocialTargets", "Social buttons for", 100)]
    public class SocialBookmarks : AbstractItem
    {
        [EditableCheckBox("Facebook", 100, ContainerName = "SocialTargets")]
        public virtual bool Facebook
        {
            get { return GetDetail("Facebook", true); }
            set { SetDetail("Facebook", value, true); }
        }

        [EditableCheckBox("Google+1", 110, ContainerName = "SocialTargets")]
        public virtual bool GooglePlus1
        {
            get { return GetDetail("GooglePlus1", true); }
            set { SetDetail("GooglePlus1", value, true); }
        }

        [EditableText(SortOrder = 110)]
        public override string Title
        {
            get { return base.Title; }
            set { base.Title = value; }
        }

        [EditableCheckBox("Like whole site", 120, HelpTitle = "This setting will make it so the whole site rather than the currently displayed page is liked")]
        public virtual bool LikeSite
        {
            get { return GetDetail("LikeSite", true); }
            set { SetDetail("LikeSite", value, true); }
        }

        protected override string TemplateName
        {
            get { return "SocialBookmarks"; }
        }
    }
}
