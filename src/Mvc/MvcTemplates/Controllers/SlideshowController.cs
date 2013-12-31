using N2.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace N2.Templates.Mvc.Controllers
{
    [Controls(typeof(N2.Web.Slideshow))]
    public class SlideshowController : TemplatesControllerBase<N2.Web.Slideshow>
    {
        public override ActionResult Index()
        {
            return PartialView(CurrentItem);
        }
    }
}
