using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2;

namespace N2.Addons.UITests.UI
{
    public partial class UITestItem : N2.Web.UI.ContentUserControl<ContentItem, Items.QueryViewerItem>
    {
        protected void OnButtonCommand(object sender, CommandEventArgs args)
        {
            ((Button) sender).Text += ".";
        }
    }
}
