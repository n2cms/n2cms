using System;
using System.Collections.Generic;
using N2.Security.Details;
using N2.Templates;
using N2.Templates.Items;
using N2.Integrity;
using N2.Web;
using N2.Definitions;

namespace N2.Addons.AddonCatalog.Items
{
    /// <summary>
    /// Since we're overriding "AbstractPage" from N2.Templates we get an editable title and a few zones for free.
    /// </summary>
    [PageDefinition("Add-On Catalog", IconUrl = Paths.UI + "plugin.png", SortOrder = 1000)]
    [RestrictParents(typeof(IStructuralPage))]
    [Template(Paths.UI + "AddonCatalogPage.aspx")]
    [Template("add", Paths.UI + "SubmitAddon.aspx")]
    public class AddonCatalog : AbstractContentPage
    {
        [EditableRoles(Title = "Role required for write access", ContainerName = Tabs.Content)]
        public virtual IEnumerable<string> ModifyRoles
        {
            get { return GetDetailCollection("ModifyRoles", true).Enumerate<string>(); }
        }
    }
}
