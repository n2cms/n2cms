using System.Web.Mvc;
using N2.Templates.Mvc.Models.Parts;
using N2.Templates.Mvc.Models;
using N2.Web;

namespace N2.Templates.Mvc.Controllers
{
    [Controls(typeof(SocialBookmarks))]
    public class SocialBookmarksController : TemplatesControllerBase<SocialBookmarks>
    {
        public override ActionResult Index()
        {
            return PartialView(new SocialBookmarksModel(CurrentItem));
        }
    }
}
