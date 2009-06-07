using System.Web.Mvc;
using MvcContrib.Filters;
using N2.Templates.Mvc.Items.Items;
using N2.Web;

namespace N2.Templates.Mvc.Controllers
{
	[ModelStateToTempData]
	[Controls(typeof(Poll))]
	public class PollController : N2Controller<Poll>
	{
		public override ActionResult Index()
		{
			return View(CurrentItem.TemplateUrl, CurrentItem);
		}

		public ActionResult Submit(int? selectedItem)
		{
			if (selectedItem == null)
				ModelState.AddModelError("Poll.Errors", "Select an alternative.");
			else
			{
				var cookie = Request.Cookies[CurrentItem.GetAnsweredCookie(selectedItem.Value).Name];

				if (cookie != null && cookie.Value != null)
					ModelState.AddModelError("Poll.Errors", "You have already voted.");
			}

			if (ModelState.IsValid)
			{
				CurrentItem.AddAnswer(Engine.Persister, selectedItem.Value);
				Response.Cookies.Add(CurrentItem.GetAnsweredCookie(selectedItem.Value));
			}
			return Redirect(CurrentItem.Parent.Url);
		}
	}
}