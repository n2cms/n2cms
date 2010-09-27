using System;
using System.Linq;
using System.Web.Mvc;
using N2.Web;
using N2.Web.Mvc;
using N2.Collections;
using N2.Templates.Mvc.Models.Pages;
using System.Collections.Generic;
using N2.Persistence.Finder;
using N2.Templates.Mvc.Models;

namespace N2.Templates.Mvc.Controllers
{
	/// <summary>
	/// This controller returns a view that displays the item created via the management interface
	/// </summary>
	[Controls(typeof(Models.Pages.NewsContainer))]
	public class NewsContainerController : ContentController<Models.Pages.NewsContainer>
	{
		IItemFinder finder;
		public NewsContainerController(IItemFinder finder)
		{
			this.finder = finder;
		}

		public override ActionResult Index()
		{
			var model = GetNews(0, 20);
			return View(model);
		}

		public ActionResult Range(int skip, int take, bool? fragment)
		{
			var model = GetNews(skip, take);

			if(fragment.HasValue && fragment.Value)
				return PartialView(model);

			return View("Index", model);
		}

		private NewsContainerModel GetNews(int skip, int take)
		{
			IList<News> news = finder.Where.Type.Eq(typeof(News))
				.And.Parent.Eq(CurrentPage)
				.FirstResult(skip)
				.MaxResults(take + 1)
				.OrderBy.Published.Desc
				.Select<News>();

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