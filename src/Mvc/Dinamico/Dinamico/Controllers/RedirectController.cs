using System;
using System.Web.Mvc;
using Dinamico.Models;
using N2.Edit;
using N2.Web;
using N2.Web.Mvc;

namespace Dinamico.Controllers
{
	[Controls(typeof(Redirect))]
	public class RedirectController : ContentController<Redirect>
	{
		public override ActionResult Index()
		{
			if (CurrentItem == null)
				return HttpNotFound();

			if (ControllerContext.HttpContext.GetViewPreference(ViewPreference.None) != ViewPreference.None)
				return View(CurrentItem);

			if (string.IsNullOrEmpty(CurrentItem.RedirectUrl))
				return View(CurrentItem);

			if (CurrentItem.RedirectPermanent)
				return RedirectPermanent(CurrentItem.RedirectUrl);

			return Redirect(CurrentItem.RedirectUrl);
		}
	}
}