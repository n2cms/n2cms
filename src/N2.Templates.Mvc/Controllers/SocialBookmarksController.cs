using System.Web.Mvc;
using N2.Templates.Mvc.Items.Items;
using N2.Templates.Mvc.Models;
using N2.Web;

namespace N2.Templates.Mvc.Controllers
{
	[Controls(typeof(SocialBookmarks))]
	public class SocialBookmarksController : N2Controller<SocialBookmarks>
	{
		public override ActionResult Index()
		{
			return PartialView(new SocialBookmarksModel(CurrentItem));
		}
	}
}