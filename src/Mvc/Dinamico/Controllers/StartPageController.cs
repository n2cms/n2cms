using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using N2.Web;
using N2.Web.Mvc;
using Dinamico.Models;
using N2.Collections;
using System.Text;
using N2;
using System.Text.RegularExpressions;
using System.Web.Security;
using N2.Engine.Globalization;

namespace Dinamico.Controllers
{
	[Controls(typeof(StartPage))]
	public class StartPageController : ContentController
    {
		public ActionResult NotFound()
		{
			return View();
		}

		public ActionResult SiteMap()
		{
			string content = Tree.From(N2.Find.StartPage)
				.Filters(new NavigationFilter())
				.ExcludeRoot(true).ToString();
			return Content(content);
		}

		public ActionResult Search(string q)
		{
			if (string.IsNullOrWhiteSpace(q))
				return Content("<li>A search term is required</li>");

			var s = Find.NH.FullText(q);

			var hits = s.SetMaxResults(50)
				.Enumerable<ContentItem>()
				.Where(h => h != null)
				.Select(h => h.IsPage ? h : h.ClosestPage())
				.Where(N2.Filters.Duplicates().Match)
				.Where(N2.Filters.Access().Match)
				.Where(N2.Filters.AncestorOrSelf(N2.Find.StartPage).Match);

			StringBuilder results = new StringBuilder();
			foreach (var hit in hits)
			{
				results.Append("<li>").Append(Link.To(hit)).Append("</li>");
			}
			
			if (results.Length == 0)
				return Content("<li>No hits</li>");

			return Content(results.ToString());
		}

		public ActionResult Translations(int id)
		{
			StringBuilder sb = new StringBuilder();

			var lg = Engine.Resolve<ILanguageGateway>();
			var translations = lg.FindTranslations(Engine.Persister.Get(id));
			foreach (var language in translations)
				sb.Append("<li>").Append(Link.To(language).Text(lg.GetLanguage(language).LanguageTitle)).Append("</li>");

			if (sb.Length == 0)
				return Content("<li>This page is not translated</li>");

			return Content(sb.ToString());
		}
	}
}
