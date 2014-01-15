using System;
using N2.Definitions;
using N2.Details;
using N2.Integrity;
using N2.Security;

namespace N2.Templates.Mvc.Models.Parts
{
    [Disable]
    [Obsolete]
    [PartDefinition("Control Panel",
        IconUrl = "~/Content/Img/controller.png",
        RequiredPermission = Permission.Administer)]
    [AllowedZones(Zones.RecursiveRight, Zones.SiteLeft, Zones.SiteRight)]
    [WithEditableTitle("Title", 10)]
    public class ControlPanel : SidebarItem
    {
    }
}
