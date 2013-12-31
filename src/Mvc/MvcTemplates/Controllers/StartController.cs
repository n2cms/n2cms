using System;
using System.Web.Mvc;
using N2.Templates.Mvc.Models.Pages;
using N2.Web;
using N2.Web.Mvc;

namespace N2.Templates.Mvc.Controllers
{
    [Controls(typeof(LanguageRoot))]
    public class StartController : TemplatesControllerBase<LanguageRoot>
    {
        public override ActionResult Index()
        {
            return View(CurrentItem);
        }

        public ActionResult NotFound()
        {
            Response.Status = "404 Not Found";
            Response.StatusCode = 404;

            StartPage startPage = CurrentPage as StartPage ?? Find.StartPage;
            if (startPage != null && startPage.NotFoundPage != null)
                return ViewPage(startPage.NotFoundPage);

            return Content("<html><body><h1>404 Not Found</h1></body></html>");
        }

        public ActionResult ServerError()
        {
            Response.Status = "500 Internal Server Error";
            Response.StatusCode = 500;

            StartPage startPage = CurrentPage as StartPage ?? Find.StartPage;
            if (startPage != null && startPage.ErrorPage != null)
                return ViewPage(startPage.ErrorPage);
            
            return Content("<html><body><h1>500 Internal Server Error</h1></body></html>");
        }
    }
}
