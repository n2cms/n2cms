using System;
using N2.Definitions;
using N2.Details;
using N2.Integrity;

namespace N2.Templates.Mvc.Models.Parts
{
	[Disable]
	[Obsolete]
	[PartDefinition("Control Panel",
		IconUrl = "~/Content/Img/controller.png")]
	[AllowedZones(Zones.RecursiveRight, Zones.SiteLeft, Zones.SiteRight)]
	[WithEditableTitle("Title", 10)]
	[ItemAuthorizedRoles("Administrators")]
	public class ControlPanel : SidebarItem
	{
	}
}