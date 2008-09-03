using N2.Definitions;
using N2.Details;
using N2.Integrity;
using N2.Templates.Items;

namespace N2.Templates.Items
{
    [Definition("Control Panel", "ControlPanel")]
    [AllowedZones(Zones.RecursiveRight, Zones.SiteLeft, Zones.SiteRight)]
    [WithEditableTitle("Title", 10)]
    [ItemAuthorizedRoles("Administrators")]
    public class ControlPanel : SidebarItem
    {
        protected override string IconName
        {
            get { return "controller"; }
        }

        protected override string TemplateName
        {
            get { return "ControlPanel"; }
        }
    }
}