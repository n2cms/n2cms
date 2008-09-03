using System;
using N2;
using N2.Resources;

namespace N2.Templates.TabPanel
{
    public partial class TabsInterface : N2.Web.UI.ContentUserControl<ContentItem, TabsItem>
    {
        protected override void OnInit(EventArgs e)
        {
            Register.JQuery(Page);
            Register.StyleSheet(Page, "~/TabPanel/TabPanel.css");
            base.OnInit(e);
        }
    }
}