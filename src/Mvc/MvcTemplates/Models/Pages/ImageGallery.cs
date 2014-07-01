using N2.Definitions;
using N2.Details;
using N2.Integrity;
using N2.Web.UI;
using System.Collections.Generic;

namespace N2.Templates.Mvc.Models.Pages
{
    [PageDefinition("Image Gallery", Description = "Displays an image with next/previous thumbnails", SortOrder = 220,
        IconClass = "fa fa-picture shadow")]
    [RestrictParents(typeof (IStructuralPage))]
    [TabContainer("images", "Gallery Images", 200)]
    public class ImageGallery : ContentPageBase
    {
        public virtual IEnumerable<GalleryItem> GalleryItems
        {
            get { return GetChildren<GalleryItem>(); }
        }
    }
}
