// This file has been deprecated. It's left here since it exists as root on many installations.

using N2.Integrity;
using N2.Installation;
using N2.Web;
using N2.Definitions;
using System;

namespace N2.Templates.Mvc.Areas.Management.Models
{
    [PageDefinition("Root Page", 
		Description = "A root page used to organize start pages.", 
		SortOrder = 0,
		InstallerVisibility = InstallerHint.PreferredRootPage,
		IconUrl = "{ManagementUrl}/Resources/icons/page_gear.png",
		TemplateUrl = "{ManagementUrl}/Myself/Root.aspx")]
    [RestrictParents(AllowedTypes.None)]
    [AvailableZone("Left", "Left")]
    [AvailableZone("Center", "Center")]
    [AvailableZone("Right", "Right")]
    [AvailableZone("Above", "Above")]
    [AvailableZone("Below", "Below")]
    [N2.Web.UI.TabContainer("smtp", "Smtp settings", 30)]
	[Obsolete]
	[Disable]
	public class RootPage : ContentItem, IRootPage
    {
        public override string Url
        {
			get { return FindPath(PathData.DefaultAction).RewrittenUrl; }
        }
    }
}