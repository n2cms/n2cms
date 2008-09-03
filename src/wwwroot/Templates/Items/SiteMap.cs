using N2.Integrity;
using N2.Templates.Items;

namespace N2.Templates.Items
{
    [N2.Definition("Site Map", "SiteMap", "Displays all pages", "", 420)]
    [RestrictParents(typeof(IStructuralPage))]
    public class SiteMap : AbstractContentPage, IStructuralPage
    {
        protected override string IconName
        {
            get { return "sitemap"; }
        }

        protected override string TemplateName
        {
            get { return "SiteMap"; }
        }
    }
}