using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Security;
using Dinamico.Models;
using N2;
using N2.Definitions;
using N2.Engine.Globalization;
using N2.Persistence.Search;
using N2.Web;
using N2.Web.Mvc;

namespace Dinamico.Controllers
{
	/// <summary>
	/// The registration <see cref="Registrations.StartPageRegistration"/> is responsible for
	/// connecting this controller to the start page model.
	/// </summary>
	public class StartPageController : ContentController<StartPage>
	{
		public ActionResult NotFound()
		{
			var closestMatch = Content.Traverse.Path(Request.AppRelativeCurrentExecutionFilePath.Trim('~', '/')).StopItem;
			
			var startPage = Content.Traverse.ClosestStartPage(closestMatch);
			var urlText = Request.AppRelativeCurrentExecutionFilePath.Trim('~', '/').Replace('/', ' ');
			var similarPages = GetSearchResults(startPage, urlText, 10).ToList();

			ControllerContext.RouteData.ApplyCurrentPath(new PathData(new ContentPage { Parent = startPage }));
			Response.TrySkipIisCustomErrors = true;
			Response.Status = "404 Not Found";

			return View(similarPages);
		}

		[ContentOutputCache]
		public ActionResult SiteMap()
		{
			var start = this.Content.Traverse.StartPage;
			string content = Tree.From(start)
				.Filters(N2.Content.Is.Accessible())
				.ExcludeRoot(true).ToString();
			return Content("<ul>" 
				+ "<li>" + Link.To(start) + "</li>"
				+ content + "</ul>");
		}

		public ActionResult Search(string q)
		{
			if (string.IsNullOrWhiteSpace(q))
				return Content("<ul><li>A search term is required</li></ul>");

			var hits = GetSearchResults(CurrentPage ?? this.Content.Traverse.StartPage, q, 50);

			StringBuilder results = new StringBuilder();
			foreach (var hit in hits)
			{
				results.Append("<li>").Append(Link.To(hit)).Append("</li>");
			}
			
			if (results.Length == 0)
				return Content("<ul><li>No hits</li></ul>");

			return Content("<ul>" + results + "</ul>");
		}

		private IEnumerable<ContentItem> GetSearchResults(ContentItem root, string text, int take)
		{
			var query = Query.For(text).Below(root).ReadableBy(User, Roles.GetRolesForUser).Except(Query.For(typeof(ISystemNode)));
			var hits = Engine.Resolve<ITextSearcher>().Search(query).Hits.Select(h => h.Content);
			return hits;
		}

		[ContentOutputCache]
		public ActionResult Translations(int id)
		{
			StringBuilder sb = new StringBuilder();

			var item = Engine.Persister.Get(id);
			var lg = Engine.Resolve<LanguageGatewaySelector>().GetLanguageGateway(item);
			var translations = lg.FindTranslations(item);
			foreach (var language in translations)
				sb.Append("<li>").Append(Link.To(language).Text(lg.GetLanguage(language).LanguageTitle)).Append("</li>");

			if (sb.Length == 0)
				return Content("<ul><li>This page is not translated</li></ul>");

			return Content("<ul>" + sb + "</ul>");
		}
	}
}
