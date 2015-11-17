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
			{
				//TODO: Maybe could search for an error page and display that instead?

				// no item to render, 404 error
				Response.StatusCode = 404;
				return new EmptyResult();
			}

			if (ControllerContext.HttpContext.GetViewPreference(N2.Edit.ViewPreference.None) != N2.Edit.ViewPreference.None)
				return View(CurrentItem.TemplateKey, CurrentItem);

			if (CurrentItem.RedirectPermanent)
				return RedirectPermanent(CurrentItem.Url);

		}
	}
}
