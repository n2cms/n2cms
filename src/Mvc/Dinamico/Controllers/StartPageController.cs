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
using N2.Definitions;

namespace Dinamico.Controllers
{
	[Controls(typeof(StartPage))]
	public class StartPageController : ContentController
    {
		public ActionResult NotFound()
		{
			return View(GetSearchResults(string.Join(" ", Request.AppRelativeCurrentExecutionFilePath.Trim('~', '/').Split('/')), 10).ToList());
		}

		[ContentOutputCache]
		public ActionResult SiteMap()
		{
			var start = N2.Find.ClosestOf<IStartPage>(CurrentPage);
			string content = Tree.From(start)
				.Filters(new NavigationFilter())
				.ExcludeRoot(true).ToString();
			return Content("<ul>" 
				+ "<li>" + Link.To(start) + "</li>"
				+ content + "</ul>");
		}

		public ActionResult Search(string q)
		{
			if (string.IsNullOrWhiteSpace(q))
				return Content("<ul><li>A search term is required</li></ul>");

			var hits = GetSearchResults(q, 50);

			StringBuilder results = new StringBuilder();
			foreach (var hit in hits)
			{
				results.Append("<li>").Append(Link.To(hit)).Append("</li>");
			}
			
			if (results.Length == 0)
				return Content("<ul><li>No hits</li></ul>");

			return Content("<ul>" + results + "</ul>");
		}

		private IEnumerable<ContentItem> GetSearchResults(string text, int take)
		{
			var s = Find.NH.FullText(text);

			var hits = s.SetMaxResults(take)
				.Enumerable<ContentItem>()
				.Where(h => h != null)
				.Select(h => h.IsPage ? h : h.ClosestPage())
				.Where(N2.Filter.Access().Match)
				.Where(N2.Filter.AncestorOrSelf(CurrentPage ?? Engine.Resolve<IWebContext>().CurrentPath.StopItem ?? N2.Find.StartPage).Match)
				.Where(N2.Filter.Duplicates().Match);
			return hits;
		}

		[ContentOutputCache]
		public ActionResult Translations(int id)
		{
			StringBuilder sb = new StringBuilder();

			var lg = Engine.Resolve<ILanguageGateway>();
			var translations = lg.FindTranslations(Engine.Persister.Get(id));
			foreach (var language in translations)
				sb.Append("<li>").Append(Link.To(language).Text(lg.GetLanguage(language).LanguageTitle)).Append("</li>");

			if (sb.Length == 0)
				return Content("<ul><li>This page is not translated</li></ul>");

			return Content("<ul>" + sb + "</ul>");
		}
	}
}
