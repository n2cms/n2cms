using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Security;
using Dinamico.Models;
using N2;
using N2.Security;
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
        private AccountManager AccountManager { get { return N2.Context.Current.Resolve<AccountManager>(); } }

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
                .Filters(N2.Content.Is.Navigatable())
                .ExcludeRoot(true).ToString();
            return Content("<li>" + Link.To(start) + "</li>" + content);
        }

        public ActionResult Search(string q)
        {
            if (string.IsNullOrWhiteSpace(q))
                return Content("<li>A search term is required</li>");

            var hits = GetSearchResults(CurrentPage ?? this.Content.Traverse.StartPage, q, 50);

            StringBuilder results = new StringBuilder();
            foreach (var hit in hits)
            {
                results.Append("<li>").Append(Link.To(hit)).Append("</li>");
            }
            
            if (results.Length == 0)
                return Content("<li>No hits</li>");

            return Content(results.ToString());
        }

        private IEnumerable<ContentItem> GetSearchResults(ContentItem root, string text, int take)
        {
            var query = Query.For(text).Below(root).ReadableBy(User, GetRolesForUser).Except(Query.For(typeof(ISystemNode)));
            var hits = Engine.Resolve<IContentSearcher>().Search(query).Hits.Select(h => h.Content);
            return hits;
        }

		protected virtual string[] GetRolesForUser(string username)
		{
            return AccountManager.GetRolesForUser(username);
            /* REMOVE:
			if (Roles.Enabled)
				return Roles.GetRolesForUser(username);
			else
				return new [] { N2.Security.AuthorizedRole.Everyone };
             */
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
                return Content("<li>This page is not translated</li>");

            return Content(sb.ToString());
        }
    }
}
