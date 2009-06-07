using N2.Collections;
using N2.Templates.Mvc.Items.Pages;
using N2.Templates.Mvc.Models;
using N2.Web;
using N2.Web.Mvc;

namespace N2.Templates.Mvc.Controllers
{
	[Controls(typeof(ImageGallery))]
	public class GalleryItemController : ContentController<ImageGallery>
	{
		public override System.Web.Mvc.ActionResult Index()
		{
			var galleryItems = CurrentItem.GetChildren(new AccessFilter(), new TypeFilter(typeof (GalleryItem)))
				.Cast<GalleryItem>();

			return View(new ImageGalleryModel(CurrentItem, galleryItems));
		}
	}
}