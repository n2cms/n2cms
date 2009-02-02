using System.Linq;
using System.Web.Mvc;
using MvcTest.Models;
using N2.Web;
using N2.Web.Mvc;
using MvcTest.Views.NewsContainer;

namespace MvcTest.Controllers
{
	[Controls(typeof(NewsContainer))]
	public class NewsContainerController : ContentController<NewsContainer>
	{
		public override ActionResult Index()
		{
			return View("Index", new NewsContainerViewData { Container = CurrentItem, News = CurrentItem.GetNews()});
		}

		public ActionResult JsonList()
		{
			var news = CurrentItem.GetNews().Select(n => new { n.Title, n.Url });
			return Json(news);
		}
	}
}
