using System;
using N2.Templates.Items;
using N2.Integrity;

namespace N2.Addons.AddonCatalog.Items
{
    /// <summary>
    /// Since we're overriding "AbstractPage" from N2.Templates we get an editable title and a few zones for free.
    /// </summary>
    [Definition("Add-On Catalog")]
    [RestrictParents(typeof(IStructuralPage))]
    public class AddonCatalog : AbstractContentPage
    {
        public string Action { get; set; }

        public override ContentItem GetChild(string childName)
        {
            if(childName.Equals("add", StringComparison.InvariantCultureIgnoreCase))
            {
                Action = "add";
                return this;
            }
            return base.GetChild(childName);
        }

        public override string TemplateUrl
        {
            get
            {
                return Paths.UI + ((Action == "add") ? "SubmitAddon.aspx" : "AddonCatalogPage.aspx");
            }
        }

        public override string IconUrl
        {
            get { return Paths.UI + "plugin.png"; }
        }
    }
}