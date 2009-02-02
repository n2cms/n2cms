using System.Web.Mvc;

namespace MvcTest.Controllers
{
	public class StaticController : Controller
	{
		public ActionResult Index()
		{
			ViewData["message"] = "An example of an 'N2-ignorant' controller.";
			ViewData["items"] = new[] {"One", "Two", "Three"};
			return View();
		}

		public ActionResult Items(string id)
		{
			ViewData["message"] = "Showing " + id;
			return View();
		}
	}
}
