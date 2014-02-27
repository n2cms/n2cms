using System;
using System.Collections.Generic;
using N2.Templates.Mvc.Models.Pages;
using N2.Web.Mvc;
using N2.Web.UI;

namespace N2.Templates.Mvc.Models
{
    public class ImageGalleryModel : IItemContainer<ImageGallery>
    {
        public ImageGalleryModel(ImageGallery imageGallery, IEnumerable<GalleryItem> galleryItems)
        {
            CurrentItem = imageGallery;
            GalleryItems = galleryItems;
        }

        public IEnumerable<GalleryItem> GalleryItems { get; private set; }

        public ImageGallery CurrentItem { get; private set; }

        /// <summary>Gets the item associated with the item container.</summary>
        ContentItem IItemContainer.CurrentItem
        {
            get { return CurrentItem; }
        }
    }
}
