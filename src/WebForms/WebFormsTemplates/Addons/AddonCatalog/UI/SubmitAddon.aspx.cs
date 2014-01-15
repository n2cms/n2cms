using System;
using System.Web.UI.WebControls;
using N2.Resources;
using N2.Templates.Web.UI;

namespace N2.Addons.AddonCatalog.UI
{
    public partial class SubmitAddon : TemplatePage<Items.AddonCatalog>
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Register.StyleSheet(this, "~/Addons/AddonCatalog/UI/AddonStyle.css");

        }
    }
}
