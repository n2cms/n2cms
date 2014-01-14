using N2.Definitions;
using N2.Details;
using N2.Integrity;
using System;
using N2.Security;

namespace N2.Templates.Items
{
    [Obsolete]
    [Disable]
    [PartDefinition("Control Panel",
        IconUrl = "~/Templates/UI/Img/controller.png",
        RequiredPermission = Permission.Administer)]
    [AllowedZones(Zones.RecursiveRight, Zones.SiteLeft, Zones.SiteRight)]
    [WithEditableTitle("Title", 10)]
    public class ControlPanel : SidebarItem
    {
        protected override string TemplateName
        {
            get { return "ControlPanel"; }
        }
    }
}
