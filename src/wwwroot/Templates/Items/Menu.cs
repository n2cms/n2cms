using N2.Integrity;
using N2.Templates.Items;
using N2.Templates.UI.Items.LayoutParts;

namespace N2.Templates.Items
{
    [Disable]
    [Definition("Menu", "Menu")]
    [RestrictParents(typeof(StartPage))] // The menu is placed on the start page and displayed on all underlying pages
    [AllowedZones(Zones.SiteLeft)]
    public class Menu : SidebarItem
    {
        public override string TemplateUrl
        {
            get { return "~/Layouts/Parts/Menu.ascx"; }
        }
    }
}