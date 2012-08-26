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
		IconUrl = "~/Content/Img/page_world.png")]
	[RestrictParents(typeof (IRootPage))]
	[AvailableZone("Site Wide Top", Zones.SiteTop), 
     AvailableZone("Site Wide Left", Zones.SiteLeft),
	 AvailableZone("Site Wide Right", Zones.SiteRight)]
	public class StartPage : LanguageRoot, IFileSystemContainer, ISitesSource
	{
		public const string LayoutArea = "layoutArea";

		// site

		[EditableLink("Not Found Page (404)", 77, ContainerName = MiscArea,
			HelpText = "Display this page when the requested URL isn't found")]
		public virtual ContentItem NotFoundPage
		{
			get { return (ContentItem) GetDetail("NotFoundPage"); }
			set { SetDetail("NotFoundPage", value); }
		}

		[EditableLink("Error Page (500)", 78, ContainerName = MiscArea,
			HelpText = "Display this page when an unhandled exception occurs.")]
		public virtual ContentItem ErrorPage
		{
			get { return (ContentItem) GetDetail("ErrorPage"); }
			set { SetDetail("ErrorPage", value); }
		}

		[EditableLink("Login Page", 79, ContainerName = MiscArea,
			HelpText = "Page to display when authorization to a page fails.")]
		public virtual ContentItem LoginPage
		{
			get { return (ContentItem) GetDetail("LoginPage"); }
			set { SetDetail("LoginPage", value); }
		}

		[EditableCheckBox("Show Breadcrumb", 110, ContainerName = LayoutArea)]
		public virtual bool ShowBreadcrumb
		{
			get { return (bool) (GetDetail("ShowBreadcrumb") ?? true); }
			set { SetDetail("ShowBreadcrumb", value, true); }
		}

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
		public virtual string HostName
		{
			get { return (string)(GetDetail("HostName") ?? string.Empty); }
			set { SetDetail("HostName", value); }
		}

		[EditableCheckBox("Site Upload", 73, ContainerName = MiscArea)]
		public virtual bool SiteUpload
		{
			get { return GetDetail("SiteUpload", false); }
			set { SetDetail("SiteUpload", value, false); }
		}

		// content

		[EditableThemeSelection("Theme", 74, ContainerName = LayoutArea)]
		public string Theme
		{
			get { return GetDetail("Theme", "Default"); }
			set { SetDetail("Theme", value, "Default"); }
		}
	}
}