using N2.Integrity;
using N2.Web.Mvc;
using N2.Definitions;

namespace N2.Templates.Mvc.Models.Pages
{
	[PageDefinition("Site Map",
		Description = "Displays all pages",
		SortOrder = 420,
		IconUrl = "~/Content/Img/sitemap.png")]
	[RestrictParents(typeof (IStructuralPage))]
	[MvcConventionTemplate]
	public class SiteMap : AbstractContentPage, IStructuralPage
	{
	}
}