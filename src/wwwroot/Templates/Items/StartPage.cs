using System.Collections.Generic;
using System.Web.UI.WebControls;
using N2.Details;
using N2.Integrity;
using N2.Web;
using N2.Web.UI;
using N2.Templates.Items;
using N2.Installation;
using N2.Engine.Globalization;
using System.Globalization;
using N2.Serialization;
using N2.Edit.FileSystem;

namespace N2.Templates.Items
{
    /// <summary>
    /// The initial page of the site.
    /// </summary>
    [Definition("Start Page", "StartPage", "A start page template. It displays a horizontal meny but no vertical menu.", "", 440, Installer = InstallerHint.PreferredRootPage | InstallerHint.PreferredStartPage)]
    [RestrictParents(typeof(RootPage), typeof(StartPage))]
    [AvailableZone("Site Wide Top", Zones.SiteTop), AvailableZone("Site Wide Left", Zones.SiteLeft), AvailableZone("Site Wide Right", Zones.SiteRight)]
    public class StartPage : AbstractStartPage, ILanguage, IFileSystemContainer
    {
        [FileAttachment, EditableImage("Top Image", 88, ContainerName = Tabs.Content, CssClass = "main")]
        public virtual string TopImage
        {
            get { return (string)(GetDetail("TopImage") ?? string.Empty); }
            set { SetDetail("TopImage", value, string.Empty); }
        }

        [FileAttachment, EditableImage("Content Image", 90, ContainerName = Tabs.Content, CssClass = "main")]
        public virtual string Image
        {
            get { return (string)(GetDetail("Image") ?? string.Empty); }
            set { SetDetail("Image", value, string.Empty); }
        }

        [EditableTextBox("Footer Text", 80, ContainerName = MiscArea, TextMode = TextBoxMode.MultiLine, Rows = 4)]
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

        [Details.ThemeSelector("Theme", 74, ContainerName = LayoutArea)]
        public override string Theme
        {
            get { return (string)(GetDetail("Theme") ?? string.Empty); }
            set { SetDetail("Theme", value); }
        }

        protected override string IconName
        {
            get { return "page_world"; }
        }

        public override string TemplateUrl
        {
            get { return "~/Default.aspx"; }
        }

        #region ILanguage Members

        public string FlagUrl
        {
            get 
            {
                if (string.IsNullOrEmpty(LanguageCode))
                    return "";
                else
                {
                    string[] parts = LanguageCode.Split('-');
                    return string.Format("~/Edit/Globalization/flags/{0}.png", parts[parts.Length - 1]);
                }
            }
        }

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
    }
}