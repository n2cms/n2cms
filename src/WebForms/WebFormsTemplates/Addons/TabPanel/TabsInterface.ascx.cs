using System;
using N2.Resources;

namespace N2.Addons.TabPanel
{
    public partial class TabsInterface : N2.Web.UI.ContentUserControl<ContentItem, TabsItem>
    {
        protected override void OnInit(EventArgs e)
        {
            Register.JQuery(Page);
            Register.StyleSheet(Page, "~/Addons/TabPanel/TabPanel.css");
            Register.JavaScript(Page, "~/Addons/TabPanel/TabPanel.js");
            base.OnInit(e);
        }
    }
}
