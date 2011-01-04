using N2.Definitions;
using N2.Details;
using N2.Integrity;
using System;

namespace N2.Templates.Items
{
	[Obsolete]
	[Disable]
	[PartDefinition("Control Panel",
		IconUrl = "~/Templates/UI/Img/controller.png")]
    [AllowedZones(Zones.RecursiveRight, Zones.SiteLeft, Zones.SiteRight)]
    [WithEditableTitle("Title", 10)]
    [ItemAuthorizedRoles("Administrators")]
    public class ControlPanel : SidebarItem
    {
        protected override string TemplateName
        {
            get { return "ControlPanel"; }
        }
    }
}