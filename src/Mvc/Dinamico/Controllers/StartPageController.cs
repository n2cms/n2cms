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
				return Content("<li>Search term is required</li>");

			StringBuilder results = new StringBuilder();
			var query = N2.Find.Items
				.Where.OpenBracket()
					.AncestralTrail.Like(Utility.GetTrail(CurrentPage) + "%")
					.Or.ID.Eq(CurrentPage.ID).CloseBracket()
				.And.OpenBracket()
					.Title.Like("%" + q + "%")
					.Or.Detail().Like("%" + q + "%").CloseBracket()
				.MaxResults(50)
				.Filters(new AccessFilter());
			var hits = query.Select().Select(i => i.IsPage ? i : i.ClosestPage()).Distinct();
			foreach (var hit in hits)
			{
				results.Append("<li>").Append(Link.To(hit)).Append("</li>");
			}
			if (results.Length == 0)
				return Content("<li>No hits</li>");

			return Content(results.ToString());
				
		}
    }
}
