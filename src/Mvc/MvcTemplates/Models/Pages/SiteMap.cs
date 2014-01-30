using N2.Integrity;
using N2.Web.Mvc;
using N2.Definitions;

namespace N2.Templates.Mvc.Models.Pages
{
    [PageDefinition("Site Map",
        Description = "Displays all pages",
        SortOrder = 420,
        IconClass = "n2-icon-sitemap")]
    [RestrictParents(typeof (IStructuralPage))]
    public class SiteMap : ContentPageBase, IStructuralPage
    {
    }
}
