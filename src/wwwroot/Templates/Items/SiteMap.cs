using N2.Integrity;

namespace N2.Templates.Items
{
    [Definition("Site Map", "SiteMap", "Displays all pages", "", 420)]
    [RestrictParents(typeof(IStructuralPage))]
	[DefaultTemplate("SiteMap")]
    public class SiteMap : AbstractContentPage, IStructuralPage
    {
        protected override string IconName
        {
            get { return "sitemap"; }
        }
    }
}