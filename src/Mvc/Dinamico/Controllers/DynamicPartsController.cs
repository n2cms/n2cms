using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using N2.Web.Mvc;
using N2.Web;

namespace Dinamico.Controllers
{
	[Controls(typeof(Models.DynamicPart))]
	public class DynamicPartsController : ContentController<Models.DynamicPart>
    {

        public override ActionResult Index()
        {
            return PartialView(CurrentItem.TemplateName, CurrentItem);
        }

    }
}
