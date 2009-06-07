using System;
using N2.Templates.Mvc.Items.Pages;
using N2.Web;

namespace N2.Templates.Mvc.Controllers
{
	[Controls(typeof(StartPage))]
	public class HomeController : N2Controller<ContentItem>
	{
	}
}