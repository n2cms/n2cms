using N2.Templates.Mvc.Models.Parts;
using N2.Web;
using N2.Web.Mvc;

namespace N2.Templates.Mvc.Controllers
{
	[Controls(typeof(NewsList))]
	public class NewsListController : ContentController<NewsList>
	{
		public override System.Web.Mvc.ActionResult Index()
		{
			if (CurrentItem.IsCentered())
				return View("List");
			else
				return View("BoxedList");
		}
	}
}