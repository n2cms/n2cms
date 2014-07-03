using System.Collections.Generic;
using N2.Details;
using N2.Integrity;
using N2.Installation;
using N2.Edit.FileSystem;
using N2.Templates.Details;
using N2.Web;
using N2.Definitions;
using N2.Edit;

namespace N2.Templates.Mvc.Models.Pages
{
    /// <summary>
    /// The initial page of the site.
    /// </summary>
    [PageDefinition("Start Page",
        Description = "A start page template. It displays a horizontal meny but no vertical menu.",
        SortOrder = 440,
        InstallerVisibility = InstallerHint.PreferredStartPage,
        IconClass = "fa fa-globe")]
    [RestrictParents(typeof (IRootPage))]
    [AvailableZone("Site Wide Top", Zones.SiteTop), 
     AvailableZone("Site Wide Left", Zones.SiteLeft),
     AvailableZone("Site Wide Right", Zones.SiteRight)]
    public class StartPage : LanguageRoot, IFileSystemContainer, ISitesSource, IThemeable
    {
        public const string LayoutArea = "layoutArea";

        // site

        [EditableLink("Not Found Page (404)", 77, ContainerName = MiscArea,
            HelpText = "Display this page when the requested URL isn't found")]
        public virtual ContentItem NotFoundPage { get; set; }

        [EditableLink("Error Page (500)", 78, ContainerName = MiscArea,
            HelpText = "Display this page when an unhandled exception occurs.")]
        public virtual ContentItem ErrorPage { get; set; }

        [EditableLink("Login Page", 79, ContainerName = MiscArea,
            HelpText = "Page to display when authorization to a page fails.")]
        public virtual ContentItem LoginPage { get; set; }

        [EditableCheckBox("Show Breadcrumb", 110, ContainerName = LayoutArea, DefaultValue = true)]
        public virtual bool ShowBreadcrumb { get; set; }

        public IEnumerable<Site> GetSites()
        {
            if (string.IsNullOrEmpty(HostName))
                return new Site[0];

            Site s = new Site((Parent ?? this).ID, ID, HostName);
            s.Wildcards = true;
            if (SiteUpload)
                s.UploadFolders.Add(new FileSystemRoot("~/Upload/" + HostName, s) { Title = "Upload (" + HostName + ")" });

            return new Site[] {s};
        }

        [EditableText("Host Name", 72, ContainerName = MiscArea)]
        public virtual string HostName { get; set; }

        [EditableCheckBox("Site Upload", 73, ContainerName = MiscArea)]
        public virtual bool SiteUpload { get; set; }

        // content

        [EditableThemeSelection("Theme", 74, ContainerName = LayoutArea, DefaultValue = "Default")]
        public virtual string Theme { get; set; }
    }
}
