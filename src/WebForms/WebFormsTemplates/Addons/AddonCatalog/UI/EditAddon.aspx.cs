using System;
using N2.Resources;
using N2.Templates.Web.UI;

namespace N2.Addons.AddonCatalog.UI
{
    public partial class EditAddon : TemplatePage<Items.Addon>
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Register.StyleSheet(this, "~/Addons/AddonCatalog/UI/AddonStyle.css");
        }
    }
}
