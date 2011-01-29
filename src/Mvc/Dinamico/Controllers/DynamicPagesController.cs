using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using N2.Web.Mvc;
using N2.Web;
using N2.Edit.FileSystem;
using System.Text;
using System.IO;
using System.Web.Routing;
using Dinamico.Models;
using N2.Web.Mvc.Html;

namespace Dinamico.Controllers
{
	[Controls(typeof(Models.DynamicPage))]
    public class DynamicPagesController : ContentController<Models.DynamicPage>
    {

        public override ActionResult Index()
        {
            return View(CurrentItem.TemplateName, CurrentItem);
        }
    }
}
