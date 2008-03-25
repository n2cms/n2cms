using System.Collections.Generic;
using System.Web.UI.WebControls;
using N2.Details;
using N2.Integrity;
using N2.Web;
using N2.Web.UI;
using N2.Templates.Items;

namespace N2.Templates.UI.Items
{
	/// <summary>
	/// The initial page of the site.
	/// </summary>
	[Definition("Start Page", "StartPage", "A start page template. It displays a horizontal meny but no vertical menu.", "", 10, MayBeRoot = true, MayBeStartPage = true)]
	[RestrictParents(typeof(RootPage))]
	[AvailableZone("Site Wide Top", Zones.SiteTop), AvailableZone("Site Wide Left", Zones.SiteLeft), AvailableZone("Site Wide Right", Zones.SiteRight)]
	[FieldSet("siteArea", "Site", 70, ContainerName = Tabs.Advanced)]
	public class StartPage : AbstractStartPage
	{
		[EditableImage("Image", 90, ContainerName = Tabs.Content, CssClass = "main")]
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

		[EditableCheckBox("Show Title", 60, ContainerName = Tabs.Advanced)]
		public virtual bool ShowTitle
		{
			get { return (bool)(GetDetail("ShowTitle") ?? true); }
			set { SetDetail("ShowTitle", value, true); }
		}

		[N2.Details.EditableItem("Header", 100, ContainerName = "siteArea")]
		public virtual LayoutParts.Top Header
		{
			get { return (LayoutParts.Top)GetDetail("Header"); }
			set { SetDetail("Header", value); }
		}

		public override string TemplateUrl
		{
			get { return "~/Default.aspx"; }
		}

		public override string IconUrl
		{
			get { return "~/Img/page_world.png"; }
		}
	}
}
