using N2.Integrity;
using N2.Web;

namespace N2.Templates.Items
{
    [Definition("Site Map", "SiteMap", "Displays all pages", "", 420)]
    [RestrictParents(typeof(IStructuralPage))]
	[ConventionTemplate]
    public class SiteMap : AbstractContentPage, IStructuralPage
    {
        protected override string IconName
        {
            get { return "sitemap"; }
        }
    }
}