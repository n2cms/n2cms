using System;
using N2.Templates.Web.UI;
using N2.Resources;

namespace N2.Addons.AddonCatalog.UI
{
    public partial class AddonPage : TemplatePage<Items.Addon>
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Register.StyleSheet(this, Paths.UI + "AddonStyle.css");
        }
    }
}
