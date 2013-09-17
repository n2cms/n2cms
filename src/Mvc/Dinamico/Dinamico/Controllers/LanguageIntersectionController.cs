using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using N2.Web.Mvc;
using Dinamico.Models;
using N2.Web;
using N2;

namespace Dinamico.Controllers
{
	[Controls(typeof(LanguageIntersection))]
	public class LanguageIntersectionController : ContentController<LanguageIntersection>
	{
		public override ActionResult Index()
		{
			ContentItem language = Request.SelectLanguage(CurrentItem);
			if (language != null)
			{
				return Redirect(language.Url);
			}

			if(CurrentItem.RedirectUrl != CurrentItem.Url)
				return Redirect(CurrentItem.RedirectUrl);

			return View(CurrentItem);
		}

	}
}
