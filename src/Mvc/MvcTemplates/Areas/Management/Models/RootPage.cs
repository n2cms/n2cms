// This file has been deprecated. It's left here since it exists as root on many installations.

using N2.Integrity;
using N2.Installation;
using N2.Web;
using N2.Definitions;
using System;
using N2.Details;
using N2.Web.UI;
using N2.Security;
using N2.Management.Api;

namespace N2.Templates.Mvc.Areas.Management.Models
{
    [PageDefinition("Root Page", 
        Description = "A root page used to organize start pages.", 
        SortOrder = 0,
        InstallerVisibility = InstallerHint.PreferredRootPage,
        IconClass = "n2-icon-sun",
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
    [RecursiveContainer("RootSettings", 120, RequiredPermission = Permission.Administer)]
    [TabContainer("Search", "Search", 120, ContainerName = "RootSettings")]
    [WithManageableSearch(ContainerName = "Search")]
    [InterfaceFlags(RemovedFlags = new [] { "Management" })]
    public class RootPage : ContentItem, IRootPage, ISystemNode
    {
        public override string Url
        {
            get { return FindPath(PathData.DefaultAction).GetRewrittenUrl(); }
        }
    }
}