using System.Globalization;
using System.Web.UI.WebControls;
using N2.Engine.Globalization;
using N2.Details;
using N2.Persistence.Serialization;
using N2.Web;
using N2.Web.UI;
using N2.Integrity;
using N2.Definitions;
using N2.Security;

namespace N2.Templates.Items
{
    //[Definition("Language root", "LanguageRoot", "A starting point for translations of the start page.", "", 450)]
    [PageDefinition("Language root",
        Description = "A starting point for translations of the start page.",
        SortOrder = 450,
        IconUrl = "~/Templates/UI/Img/page_world.png")]
    [RecursiveContainer("SiteAreaContainer", 70,
        RequiredPermission = Permission.Administer)]
    [TabContainer(LanguageRoot.SiteArea, "Site", 0, 
        ContainerName = "SiteAreaContainer")]
    [RestrictParents(typeof(StartPage))]
    [FieldSetContainer(StartPage.MiscArea, "Miscellaneous", 80, ContainerName = LanguageRoot.SiteArea)]
    [FieldSetContainer(StartPage.LayoutArea, "Layout", 75, ContainerName = LanguageRoot.SiteArea)]
    [ConventionTemplate("Start")]
    public class LanguageRoot : AbstractContentPage, IStartPage, IStructuralPage, ILanguage
    {
        public LanguageRoot()
        {
            Visible = false;
            SortOrder = 10000;
        }

        public const string SiteArea = "siteArea";
        public const string MiscArea = "miscArea";

        #region ILanguage Members

        [EditableLanguagesDropDown("Language", 100, ContainerName = MiscArea)]
        public string LanguageCode
        {
            get { return (string)GetDetail("LanguageCode"); }
            set { SetDetail("LanguageCode", value); }
        }

        public string LanguageTitle
        {
            get
            {
                if (string.IsNullOrEmpty(LanguageCode))
                    return "";
                else
                    return new CultureInfo(LanguageCode).DisplayName;
            }
        }

        #endregion


        [FileAttachment, EditableFileUploadAttribute("Top Image", 88, ContainerName = Tabs.Content, CssClass = "main")]
        public virtual string TopImage
        {
            get { return (string)(GetDetail("TopImage") ?? string.Empty); }
            set { SetDetail("TopImage", value, string.Empty); }
        }

        [FileAttachment, EditableFileUploadAttribute("Content Image", 90, ContainerName = Tabs.Content, CssClass = "main")]
        public virtual string Image
        {
            get { return (string)(GetDetail("Image") ?? string.Empty); }
            set { SetDetail("Image", value, string.Empty); }
        }

        [EditableText("Footer Text", 80, ContainerName = MiscArea, TextMode = TextBoxMode.MultiLine, Rows = 3)]
        public virtual string FooterText
        {
            get { return (string)(GetDetail("FooterText") ?? string.Empty); }
            set { SetDetail("FooterText", value, string.Empty); }
        }

        [EditableItem("Header", 100, ContainerName = SiteArea)]
        public virtual Top Header
        {
            get { return (Top)GetDetail("Header"); }
            set { SetDetail("Header", value); }
        }
    }
}
