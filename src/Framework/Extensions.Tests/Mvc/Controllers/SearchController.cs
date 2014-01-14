using System;
using System.Web.Mvc;
using N2.Extensions.Tests.Mvc.Models;
using N2.Web;
using N2.Web.Mvc;

namespace N2.Extensions.Tests.Mvc.Controllers
{
    [Controls(typeof(SearchPage))]
    public class SearchController : ContentController<SearchPage>
    {
        public override ActionResult Index()
        {
            throw new NotImplementedException();
        }

        public ActionResult Find(string q)
        {
            return View(new string[q.Length]);
        }
    }
}
