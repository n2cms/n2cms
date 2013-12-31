using N2.Integrity;
using N2.Details;
using N2.Web;
using N2.Web.UI;
using N2.Definitions;

namespace N2.Templates.Items
{
    [PageDefinition("Image Gallery", Description = "Displays an image with next/previous thumbnails", SortOrder = 220,
        IconUrl = "~/Templates/UI/Img/photos.png")]
    [RestrictParents(typeof(IStructuralPage))]
    [FieldSetContainer(ImageGallery.GallerySettings, "Gallery Settings", 500, ContainerName = Tabs.Content)]
    [ConventionTemplate]
    public class ImageGallery : AbstractContentPage
    {
        #region GallerySettings
        public const string GallerySettings = "gallerySettings";

        [EditableNumber("Max Image Width", 200, ContainerName = GallerySettings)]
        public virtual int MaxImageWidth
        {
            get { return (int)(GetDetail("MaxImageWidth") ?? 685); }
            set { SetDetail("MaxImageWidth", value); }
        }

        [EditableNumber("Max Image Height", 210, ContainerName = GallerySettings)]
        public virtual int MaxImageHeight
        {
            get { return (int)(GetDetail("MaxImageHeight") ?? 685); }
            set { SetDetail("MaxImageHeight", value); }
        }

        [EditableNumber("Max Thumbnail Width", 220, ContainerName = GallerySettings)]
        public virtual int MaxThumbnailWidth
        {
            get { return (int)(GetDetail("MaxThumbnailWidth") ?? 70); }
            set { SetDetail("MaxThumbnailWidth", value); }
        }

        [EditableNumber("Max Thumbnail Height", 230, ContainerName = GallerySettings)]
        public virtual int MaxThumbnailHeight
        {
            get { return (int)(GetDetail("MaxThumbnailHeight") ?? 60); }
            set { SetDetail("MaxThumbnailHeight", value); }
        } 
        #endregion
    }
}
