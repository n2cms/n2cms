using MvcTest.Models;
using N2.Web;
using N2.Web.Mvc;

namespace MvcTest.Controllers
{
	[Controls(typeof(AbstractPage))]
	public class ContentController : ContentController<AbstractPage>
	{
	}
}
