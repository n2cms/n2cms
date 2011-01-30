using System.Web.Mvc;
using N2.Web;
using N2.Web.Mvc;

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
