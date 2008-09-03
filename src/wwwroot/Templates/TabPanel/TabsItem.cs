using N2.Templates.Items;

namespace N2.Templates.TabPanel
{
    [Definition]
    public class TabsItem : TextItem
    {
        public override string TemplateUrl
        {
            get { return "~/TabPanel/TabsInterface.ascx"; }
        }

        public override string IconUrl
        {
            get { return "~/Img/tab.png"; }
        }
    }
}