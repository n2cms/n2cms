using System;
using System.Globalization;
using System.Web.UI.WebControls;
using N2.Engine.Globalization;
using N2.Details;
using N2.Persistence.Serialization;
using N2.Templates.Mvc.Models.Parts;
using N2.Web.UI;
using N2.Integrity;
using N2.Definitions;
using N2.Security;
using N2.Web.Parts;

namespace N2.Templates.Mvc.Models.Pages
{
    [PageDefinition("Language root",
        Description = "A starting point for translations of the start page.",
        SortOrder = 450,
        IconClass = "fa fa-globe")]
    [RecursiveContainer(LanguageRoot.SiteArea, 70, 
        RequiredPermission = Permission.Administer)]
    [RestrictParents(typeof (StartPage))]
    [TabContainer(StartPage.LayoutArea, "Layout", 75, ContainerName = LanguageRoot.SiteArea)]
    [TabContainer(StartPage.MiscArea, "Miscellaneous", 80, ContainerName = LanguageRoot.SiteArea)]
    [TabContainer("top", "Top", 100, ContainerName = LanguageRoot.SiteArea)]
    public class LanguageRoot : ContentPageBase, IStructuralPage, ILanguage, IStartPage
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
            get { return (string) GetDetail("LanguageCode"); }
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

        [FileAttachment, EditableImageUpload("Top Image", 88, ContainerName = Tabs.Content, CssClass = "top")]
        public virtual string TopImage { get; set; }

        [EditableMedia("Content Image", 90, ContainerName = Tabs.Content, CssClass = "main")]
        public virtual string Image { get; set; }

        [EditableText("Footer Text", 80, ContainerName = MiscArea, TextMode = TextBoxMode.MultiLine, Rows = 3)]
        public virtual string FooterText { get; set; }

        [EditableItem("Header", 100, ContainerName = "Top")]
        public virtual Top Header
        {
            get { return this.LoadEmbeddedPart<Top>("Header"); }
            set { this.StoreEmbeddedPart("Header", value); }
        }
    }
}
