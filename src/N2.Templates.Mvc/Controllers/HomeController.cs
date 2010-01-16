using System;
using System.Web.Mvc;
using N2.Templates.Mvc.Models.Pages;
using N2.Web;
using N2.Web.Mvc;

namespace N2.Templates.Mvc.Controllers
{
	[Controls(typeof(StartPage))]
	public class HomeController : TemplatesControllerBase<StartPage>
	{
		public ActionResult NotFound()
		{
			Response.Status = "404 Not Found";
			Response.StatusCode = 404;

			if (CurrentItem.NotFoundPage == null)
				return Content("<html><body><h1>404 Not Found</h1></body></html>");

			return ViewPage(CurrentItem.NotFoundPage);
		}

		public ActionResult ServerError()
		{
			Response.Status = "500 Internal Server Error";
			Response.StatusCode = 500;

			if (CurrentItem.ErrorPage == null)
				return Content("<html><body><h1>500 Internal Server Error</h1></body></html>");

			return ViewPage(CurrentItem.ErrorPage);
		}
	}
}