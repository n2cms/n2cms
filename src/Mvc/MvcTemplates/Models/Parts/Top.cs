using N2.Integrity;
using N2.Details;
using N2.Definitions;
using N2.Persistence.Serialization;
using N2.Templates.Mvc.Models.Pages;

namespace N2.Templates.Mvc.Models.Parts
{
    [Disable]
    [PartDefinition("Top",
        IconUrl = "~/Content/Img/page_white_star.png")]
    [RestrictParents(typeof (LanguageRoot))]
    // The top region is placed on the start page and displayed on all underlying pages
    [AllowedZones("SiteTop")]
    public class Top : PartBase
    {
        [DisplayableHeading(2)]
        [EditableText("Top text", 40)]
        public override string Title
        {
            get { return base.Title; }
            set { base.Title = value; }
        }

        [EditableUrl("Top text url", 42)]
        public virtual string TopTextUrl
        {
            get
            {
                string _url = (string) GetDetail("TopTextUrl");
                return string.IsNullOrEmpty(_url) ? "~/" : _url;
            }
            set { SetDetail("TopTextUrl", value, "~/"); }
        }

        [FileAttachment, EditableFileUploadAttribute("Logo", 50, Alt = "Logo")]
        public virtual string LogoUrl
        {
            get { return (string) (GetDetail("LogoUrl") ?? string.Empty); }
            set { SetDetail("LogoUrl", value); }
        }

        [EditableUrl("Logo url", 52)]
        public virtual string LogoLinkUrl
        {
            get { return (string) (GetDetail("LogoLinkUrl") ?? "/"); }
            set { SetDetail("LogoLinkUrl", value, "/"); }
        }
    }
}
