using N2.Integrity;
using N2.Web;
using N2.Definitions;

namespace N2.Templates.Items
{
    [PageDefinition("Site Map", 
        Description = "Displays all pages",
        SortOrder = 420,
        IconUrl = "~/Templates/UI/Img/sitemap.png")]
    [RestrictParents(typeof(IStructuralPage))]
    [ConventionTemplate]
    public class SiteMap : AbstractContentPage, IStructuralPage
    {
    }
}
