using N2.Collections;
using N2.Templates.Mvc.Models.Pages;
using N2.Templates.Mvc.Models;
using N2.Web;
using N2.Web.Mvc;
using System.Web.Mvc;

namespace N2.Templates.Mvc.Controllers
{
    [Controls(typeof(ImageGallery))]
    public class ImageGalleryController : ContentController<ImageGallery>
    {
        public override System.Web.Mvc.ActionResult Index()
        {
            var galleryItems = CurrentItem.GalleryItems;

            return View(new ImageGalleryModel(CurrentItem, galleryItems));
        }
    }

}
