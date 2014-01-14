using System;
using System.Linq;
using System.Web.Mvc;
using N2.Definitions;
using N2.Web;
using N2.Web.Mvc;
using N2.Collections;
using N2.Templates.Mvc.Models.Pages;
using System.Collections.Generic;
using N2.Persistence.Finder;
using N2.Templates.Mvc.Models;
using N2.Persistence;

namespace N2.Templates.Mvc.Controllers
{
    /// <summary>
    /// This controller returns a view that displays the item created via the management interface
    /// </summary>
    [Controls(typeof(Models.Pages.NewsContainer))]
    [GroupChildren(GroupChildrenMode.PublishedYearMonth)]
    public class NewsContainerController : ContentController<Models.Pages.NewsContainer>
    {
        IContentItemRepository finder;
        public NewsContainerController(IContentItemRepository finder)
        {
            this.finder = finder;
        }

        [NonAction]
        public override ActionResult Index()
        {
            return base.Index();
        }

        public virtual ActionResult Index(string tag)
        {
            var model = string.IsNullOrEmpty(tag) ? GetNews(0, 20) : GetNews(tag, 0, 20);
            return View(model);
        }

        public ActionResult Range(int skip, int take, bool? fragment, string tag)
        {
            var model = string.IsNullOrEmpty(tag)
                ? GetNews(skip, take)
                : GetNews(tag, skip, take);

            if(fragment.HasValue && fragment.Value)
                return PartialView(model);

            return View("Index", model);
        }

        private NewsContainerModel GetNews(string tag, int skip, int take)
        {
            //IList<News> news = finder.Where.Type.Eq(typeof(News))
            //  .And.Parent.Eq(CurrentPage)
            //  .And.Detail("Tags").Like(tag)
            //  .FirstResult(skip)
            //  .MaxResults(take + 1)
            //  .OrderBy.Published.Desc
            //  .Select<News>();
            var query = (Parameter.Below(CurrentPage) & Parameter.Like("Tags", tag).Detail()).Skip(skip).Take(take + 1).OrderBy("Published DESC");
            var news = finder.Find(query).OfType<News>().ToList();
            var model = CreateModel(skip, take, news);
            model.Tag = tag;
            return model;
        }

        private NewsContainerModel GetNews(int skip, int take)
        {
            var query = Parameter.Below(CurrentPage).Skip(skip).Take(take + 1).OrderBy("Published DESC");
            var news = finder.Find(query).OfType<News>().ToList();
            //IList<News> news = finder.Where.Type.Eq(typeof(News))
            //  .And.Parent.Eq(CurrentPage)
            //  .FirstResult(skip)
            //  .MaxResults(take + 1)
            //  .OrderBy.Published.Desc
            //  .Select<News>();

            return CreateModel(skip, take, news);
        }

        private NewsContainerModel CreateModel(int skip, int take, IList<News> news)
        {
            var model = new NewsContainerModel
            {
                Container = CurrentItem,
                News = news,
                Skip = skip,
                Take = take,
                IsLast = news.Count <= take
            };
            if (!model.IsLast)
                model.News.RemoveAt(model.News.Count - 1);

            return model;
        }
    }
}
