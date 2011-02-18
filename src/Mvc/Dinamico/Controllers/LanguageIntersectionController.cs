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

			ContentItem language = SelectTranslations();
			if (language != null)
				return ViewPage(language);

			return View(CurrentPage);
        }

		private ContentItem SelectTranslations()
		{
			// Picks the translation best matching the browser-language or the first translation in the list

			var translations = GetTranslations().ToList();
			
			if (Request.UserLanguages == null)
				return translations.FirstOrDefault();

			var selectedlanguage = Request.UserLanguages.Select(ul => translations.FirstOrDefault(t => t.LanguageCode == ul))
				.Where(t => t != null)
				.FirstOrDefault();
			return selectedlanguage ?? translations.FirstOrDefault();
		}

		private IEnumerable<StartPage> GetTranslations()
		{
			return CurrentPage.GetChildren().OfType<StartPage>();
		}

    }
}
