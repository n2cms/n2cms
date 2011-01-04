using N2.Details;
using N2.Integrity;
using N2.Installation;
using N2.Web;
using N2.Web.UI;
using N2.Definitions;

namespace N2.Management.Myself
{
    [PageDefinition("Root Page (fallback)", 
		Description = "A fallback root page used to organize start pages. This root can be replaced or inherited in a web application project.", 
		SortOrder = 0,
		InstallerVisibility = InstallerHint.PreferredRootPage,
		IconUrl = "{ManagementUrl}/Resources/icons/page_gear.png",
		TemplateUrl = "{ManagementUrl}/Myself/Root.aspx")]
    [RestrictParents(AllowedTypes.None)]
	[AllowedChildren(typeof(ContentItem))]
    [AvailableZone("Left", "Left")]
    [AvailableZone("Center", "Center")]
    [AvailableZone("Right", "Right")]
    [AvailableZone("Above", "Above")]
    [AvailableZone("Below", "Below")]
	public class RootBase : ContentItem, IRootPage
    {
        public override string Url
        {
			get { return FindPath(PathData.DefaultAction).RewrittenUrl; }
        }
    }
}