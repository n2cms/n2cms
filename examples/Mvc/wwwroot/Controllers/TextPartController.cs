using System;
using System.Web.Mvc;
using N2.Web;
using N2.Web.Mvc;

namespace MvcTest.Controllers
{
	/// <summary>
	/// This controller returns a view that displays the item created via the management interface
	/// </summary>
	[Controls(typeof(Models.TextPart))]
	public class TextPartController : ContentController<Models.TextPart>
	{
		public override ActionResult Index()
		{
			// Right-click and Add View..
			return View(CurrentItem); // Passing the current content item is optional
		}
	}
}