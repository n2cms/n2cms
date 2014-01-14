using N2.Integrity;
using N2.Templates;
using N2.Templates.Items;

namespace N2.Addons.TabPanel
{
    [PartDefinition("Tabs", TemplateUrl = "~/Addons/TabPanel/TabsInterface.ascx", IconUrl = "~/Addons/TabPanel/tab.png")]
    [AllowedZones(Zones.Content)]
    public class TabsItem : TextItem
    {
    }
}
