#if DEBUG
using N2.Web;
using N2.Web.Mvc;
using System.Web.Mvc;

namespace N2.Templates.Mvc.Areas.Tests.Controllers
{
    [Controls(typeof(Models.PageInfoViewerPart))]
    public class PageInfoViewerController : ContentController<Models.PageInfoViewerPart>
    {
        //
        // GET: /Tests/PageInfoViewer/

        public override ActionResult Index()
        {
            if (!CurrentItem.VisibleToEveryone && !Engine.SecurityManager.IsEditor(User))
                return Content("");
            
            return View();
        }
    }
}
#endif
