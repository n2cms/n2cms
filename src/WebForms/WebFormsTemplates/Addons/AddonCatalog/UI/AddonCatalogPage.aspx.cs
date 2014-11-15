using System;
using System.Web.UI.WebControls;
using N2.Resources;
using N2.Templates.Web.UI;
using N2.Collections;

namespace N2.Addons.AddonCatalog.UI
{
    public partial class AddonCatalogPage : TemplatePage<Items.AddonCatalog>
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Register.StyleSheet(this, Paths.UI + "AddonStyle.css");

            ItemList<ContentItem> addons = CurrentPage.GetChildPagesUnfiltered().Where(new AllFilter(new AccessFilter(), new TypeFilter(typeof (Items.Addon))));
            foreach(Items.Addon addon in addons)
            {
                AddonSummary uc = LoadControl("AddonSummary.ascx") as AddonSummary;
                uc.CurrentItem = addon;
                phAddons.Controls.Add(uc);
            }

            DataBind();
        }
    }
}
