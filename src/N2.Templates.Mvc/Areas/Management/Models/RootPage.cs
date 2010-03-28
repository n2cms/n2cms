using N2.Details;
using N2.Integrity;
using N2.Installation;
using N2.Web;
using N2.Web.UI;
using N2.Definitions;

namespace N2.Templates.Mvc.Areas.Management.Models
{
    [PageDefinition("Root Page", 
		Description = "A root page used to organize start pages.", 
		SortOrder = 0,
		InstallerVisibility = InstallerHint.PreferredRootPage,
		IconUrl = "~/N2/Resources/icons/page_gear.png",
		TemplateUrl = "~/Areas/Management/Root.aspx")]
    [RestrictParents(AllowedTypes.None)]
    [AvailableZone("Left", "Left")]
    [AvailableZone("Center", "Center")]
    [AvailableZone("Right", "Right")]
    [AvailableZone("Above", "Above")]
    [AvailableZone("Below", "Below")]
    [N2.Web.UI.TabContainer("smtp", "Smtp settings", 30)]
	public class RootPage : ContentItem, IRootPage
    {
        [EditableTextBox("Smtp Host", 100, ContainerName = "smtp")]
        public virtual string SmtpHost
        {
            get { return (string)(GetDetail("SmtpHost") ?? string.Empty); }
            set { SetDetail("SmtpHost", value, string.Empty); }
        }

        [EditableTextBox("Smtp Port", 110, ContainerName = "smtp")]
        public virtual int SmtpPort
        {
            get { return (int)(GetDetail("SmtpPort") ?? 25); }
            set { SetDetail("SmtpPort", value, 25); }
        }

        [EditableTextBox("Smtp User", 100, ContainerName = "smtp")]
        public virtual string SmtpUser
        {
            get { return (string)(GetDetail("SmtpUser") ?? string.Empty); }
            set { SetDetail("SmtpUser", value, string.Empty); }
        }

        [EditableTextBox("Smtp Password", 100, ContainerName = "smtp")]
        public virtual string SmtpPassword
        {
            get { return (string)(GetDetail("SmtpPassword") ?? string.Empty); }
            set { SetDetail("SmtpPassword", value, string.Empty); }
        }

        public override string Url
        {
			get { return FindPath(PathData.DefaultAction).RewrittenUrl; }
        }
    }
}