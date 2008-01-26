using System.Collections.Generic;
using System.Web.UI.WebControls;
using N2.Details;
using N2.Integrity;
using N2.Web;
using N2.Web.UI;

namespace N2.Templates.Items
{
	/// <summary>
	/// The initial page of the site.
	/// </summary>
	[Definition("Start Page", "StartPage", "A start page template. It displays a horizontal meny but no vertical menu.", "", 10, MayBeRoot = true, MayBeStartPage = true)]
	[RestrictParents(typeof(RootPage))]
	[AvailableZone("Site Wide Top", "SiteTop"),AvailableZone("Site Wide Left", "SiteLeft"),AvailableZone("Site Wide Right", "SiteRight")]
	[FieldSet("siteArea", "Site", 70, ContainerName = "advanced")]
	public class StartPage : AbstractContentPage, IStructuralPage, ISitesSource
	{
		[EditableImage("Image", 90, ContainerName = "content", CssClass = "main")]
		public virtual string Image
		{
			get { return (string)(GetDetail("Image") ?? string.Empty); }
			set { SetDetail("Image", value, string.Empty); }
		}

		[EditableTextBox("Footer Text", 80, ContainerName = "siteArea", TextMode = TextBoxMode.MultiLine, Rows = 4)]
		public virtual string FooterText
		{
			get { return (string)(GetDetail("FooterText") ?? string.Empty); }
			set { SetDetail("FooterText", value, string.Empty); }
		}

		[EditableCheckBox("Show Title", 60, ContainerName = "advanced")]
		public virtual bool ShowTitle
		{
			get { return (bool)(GetDetail("ShowTitle") ?? true); }
			set { SetDetail("ShowTitle", value, true); }
		}

		[EditableTextBox("Host Name", 72, ContainerName = "siteArea")]
		public virtual string HostName
		{
			get { return (string)(GetDetail("HostName") ?? string.Empty); }
			set { SetDetail("HostName", value); }
		}

		[Details.ThemeSelector("Theme", 74, ContainerName = "siteArea")]
		public virtual string Theme
		{
			get { return (string)(GetDetail("Theme") ?? string.Empty); }
			set { SetDetail("Theme", value); }
		}

		[Details.LayoutSelector("Layout", 76, ContainerName = "siteArea")]
		public virtual string Layout
		{
			get { return (string)(GetDetail("Layout") ?? "~/Layouts/DefaultLayout.Master"); }
			set { SetDetail("Layout", value); }
		}

		public override string TemplateUrl
		{
			get { return "~/Default.aspx"; }
		}

		public override string IconUrl
		{
			get { return "~/Img/page_world.png"; }
		}

		public IEnumerable<Site> GetSites()
		{
			yield return new Site(Parent.ID, ID, HostName);
			yield return new Site(Parent.ID, ID, "www." + HostName);
		}
	}
}
