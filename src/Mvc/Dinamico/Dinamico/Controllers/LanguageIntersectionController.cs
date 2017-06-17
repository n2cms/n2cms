using System;
using System.Web.Mvc;
using Dinamico.Models;
using N2.Web;
using N2.Web.Mvc;

namespace Dinamico.Controllers
{
	[Controls(typeof(LanguageIntersection))]
	public class LanguageIntersectionController : ContentController<LanguageIntersection>
	{
		public override ActionResult Index()
		{
			var language = Request.SelectLanguage(CurrentItem);
			if (language != null)
			{
				if (language.Url.StartsWith("http"))
					return Redirect(language.Url);

				if (language.Url != CurrentPage.Url)
					return Redirect(language.Url);

				return ViewPage(language);
			}

			if (CurrentItem.RedirectUrl != CurrentItem.Url)
				return Redirect(CurrentItem.RedirectUrl);

			return View(CurrentItem);
		}
	}
}