using System.Linq;
using N2.Templates.Mvc.Models.Pages;
using N2.Templates.Mvc.Models.Parts;
using N2.Web;
using N2.Web.Mvc;
using N2.Collections;

namespace N2.Templates.Mvc.Controllers
{
	[Controls(typeof(NewsList))]
	public class NewsListController : ContentController<NewsList>
	{
		public override System.Web.Mvc.ActionResult Index()
		{
			string viewName = CurrentItem.Boxed ? "BoxedList" : "List";

			ContentItem root = CurrentItem.Container ?? N2.Find.Closest<LanguageRoot>(CurrentPage) ?? N2.Find.StartPage;
			if(root == null)
				return View(viewName, Enumerable.Empty<News>());

			var news = N2.Find.Items.Where.Type.Eq(typeof(News))
				.And.AncestralTrail.Like(Utility.GetTrail(root) + "%")
				.OrderBy.Published.Desc
				.Filters(new AccessFilter(), new PublishedFilter())
				.MaxResults(CurrentItem.MaxNews)
				.Select<News>();

			return View(viewName, news);
		}
	}
}