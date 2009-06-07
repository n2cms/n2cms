using System.Web.Mvc;
using N2.Templates.Mvc.Items.Pages;
using N2.Web;
using N2.Web.Mvc;

namespace N2.Templates.Mvc.Controllers
{
	[Controls(typeof(RootPage))]
	[Authorize(Users = "admin")]//, Roles = "Administrators,Editors,Writers")]
	public class RootPageController : ContentController<RootPage>
	{
		public override ActionResult Index()
		{
			return View("Index", CurrentItem);
		}
	}
}