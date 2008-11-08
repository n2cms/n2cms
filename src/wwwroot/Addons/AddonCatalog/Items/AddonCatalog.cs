using System;
using N2.Templates.Items;
using N2.Integrity;
using N2.Web;

namespace N2.Addons.AddonCatalog.Items
{
    /// <summary>
    /// Since we're overriding "AbstractPage" from N2.Templates we get an editable title and a few zones for free.
    /// </summary>
    [Definition("Add-On Catalog", SortOrder = 1000)]
    [RestrictParents(typeof(IStructuralPage))]
	[Template(Paths.UI + "AddonCatalogPage.aspx")]
	[Template("add", Paths.UI + "SubmitAddon.aspx")]
    public class AddonCatalog : AbstractContentPage
    {
        public override string IconUrl
        {
            get { return Paths.UI + "plugin.png"; }
        }
    }
}