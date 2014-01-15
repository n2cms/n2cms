using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using N2.Web.Mvc;
using N2.Web;

namespace Dinamico.Controllers
{
    [Controls(typeof(Models.ContentPart))]
    public class ContentPartsController : ContentController<Models.ContentPart>
    {

        public override ActionResult Index()
        {
            return PartialView((string)CurrentItem.TemplateKey, CurrentItem);
        }

    }

    [Controls(typeof(Slideshow))]
    public class SlideshowController : ContentController<Slideshow>
    {
        public override ActionResult Index()
        {
            return PartialView((string)CurrentItem.TemplateKey, CurrentItem);
        }
    }

}
