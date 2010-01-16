using N2.Templates.Mvc.Models.Parts;
using N2.Web;
using N2.Web.Mvc;

namespace N2.Templates.Mvc.Controllers
{
	[Controls(typeof(NewsList))]
	public class NewsListController : ContentController<NewsList>
	{

	}
}