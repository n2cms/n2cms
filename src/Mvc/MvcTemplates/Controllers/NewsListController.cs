using System.Linq;
using N2.Templates.Mvc.Models.Pages;
using N2.Templates.Mvc.Models.Parts;
using N2.Web;
using N2.Web.Mvc;
using N2.Collections;
using N2.Linq;
using N2.Persistence;
using N2.Security;

namespace N2.Templates.Mvc.Controllers
{
    [Controls(typeof(NewsList))]
    public class NewsListController : ContentController<NewsList>
    {
        private IContentItemRepository repository;

        public NewsListController(IContentItemRepository repository)
        {
            this.repository = repository;
        }

        public override System.Web.Mvc.ActionResult Index()
        {
            string viewName = CurrentItem.Boxed ? "BoxedList" : "List";

            ContentItem root = CurrentItem.Container ?? N2.Find.Closest<LanguageRoot>(CurrentPage) ?? N2.Find.StartPage;
            if(root == null)
                return View(viewName, Enumerable.Empty<News>());

            //var news = N2.Find.Items.Where.Type.Eq(typeof(News))
            //  .And.AncestralTrail.Like(Utility.GetTrail(root) + "%")
            //  .OrderBy.Published.Desc
            //  .Filters(new AccessFilter(), new PublishedFilter())
            //  .MaxResults(CurrentItem.MaxNews)
            //  .Select<News>();
            var parameters = Parameter.Below(root) & Parameter.State(ContentState.Published) & Parameter.TypeEqual(typeof(News).Name);
            var news = repository.Find(parameters.Take(CurrentItem.MaxNews).OrderBy("Published DESC"))
                .OfType<News>().ToList();
            
            return View(viewName, news.Where(Content.Is.Accessible()));
        }
    }
}
