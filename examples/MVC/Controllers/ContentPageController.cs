using N2.Web;
using N2.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC.Controllers
{
    [Controls(typeof(Models.ContentPage))]
    public class ContentPageController : ContentController<Models.ContentPage>
    {
        public override System.Web.Mvc.ActionResult Index()
        {
            return base.Index();
        }
    }
}
