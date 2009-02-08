using N2.Integrity;
using N2.Details;
using N2.Definitions;
using N2.Serialization;

namespace N2.Templates.Items
{
    [Disable]
    [Definition("Top", "Top")]
    [N2.Web.UI.FieldSetContainer("top", "Top", 100)]
    [RestrictParents(typeof(LanguageRoot))] // The top region is placed on the start page and displayed on all underlying pages
    [AllowedZones("SiteTop")]
    public class Top : AbstractItem
    {
        [Displayable(typeof(N2.Web.UI.WebControls.H2), "Text")]
        [EditableTextBox("Top text", 40, ContainerName = "top")]
        public override string Title
        {
            get { return base.Title; }
            set { base.Title = value; }
        }

        [EditableUrl("Top text url", 42, ContainerName = "top")]
        public virtual string TopTextUrl
        {
            get {
        		string _url = (string)GetDetail("TopTextUrl");
        		return string.IsNullOrEmpty(_url) ? "~/" : _url;
        	}
            set { SetDetail("TopTextUrl", value, "~/"); }
        }

        [FileAttachment, EditableImage("Logo", 50, ContainerName = "top", Alt = "Logo")]
        public virtual string LogoUrl
        {
            get { return (string)(GetDetail("LogoUrl") ?? string.Empty); }
            set { SetDetail("LogoUrl", value); }
        }

        [EditableUrl("Logo url", 52, ContainerName = "top")]
        public virtual string LogoLinkUrl
        {
            get { return (string)(GetDetail("LogoLinkUrl") ?? "/"); }
            set { SetDetail("LogoLinkUrl", value, "/"); }
        }

        protected override string IconName
        {
            get { return "page_white_star"; }
        }

        protected override string TemplateName
        {
            get { return "Top"; }
        }
    }
}
