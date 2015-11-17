using System;
using System.Web.Mvc;
using N2.Web;
using N2.Web.Mvc;

namespace Dinamico.Controllers
{
	[Controls(typeof(Models.Redirect))]
	public class RedirectController : ContentController<Models.Redirect>
	{
		public override ActionResult Index()
		{
			if (CurrentItem == null)
				return HttpNotFound();

			if (ControllerContext.HttpContext.GetViewPreference(N2.Edit.ViewPreference.None) != N2.Edit.ViewPreference.None)
				return View(CurrentItem);

			if (string.IsNullOrEmpty(CurrentItem.RedirectUrl))
				return View(CurrentItem);

			if (CurrentItem.RedirectPermanent)
				return RedirectPermanent(CurrentItem.RedirectUrl);

			return Redirect(CurrentItem.RedirectUrl);
		}
	}
}
