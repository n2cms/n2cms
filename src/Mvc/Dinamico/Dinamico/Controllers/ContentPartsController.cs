using System;
using System.Web.Mvc;
using Dinamico.Models;
using N2.Web;
using N2.Web.Mvc;

namespace Dinamico.Controllers
{
	[Controls(typeof(ContentPart))]
	public class ContentPartsController : ContentController<ContentPart>
	{
		public override ActionResult Index()
		{
			return PartialView(CurrentItem.TemplateKey, CurrentItem);
		}
	}

	[Controls(typeof(Slideshow))]
	public class SlideshowController : ContentController<Slideshow>
	{
		public override ActionResult Index()
		{
			return PartialView(CurrentItem.TemplateKey, CurrentItem);
		}
	}
}