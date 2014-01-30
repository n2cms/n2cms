using N2.Integrity;
using N2.Details;
using N2.Web;
using N2.Web.UI;
using N2.Persistence.Serialization;

namespace N2.Templates.Items
{
    [PageDefinition("Gallery Item",
        IconUrl = "~/Templates/UI/Img/photo.png")]
    [RestrictParents(typeof(ImageGallery))]
    [TabContainer("advanced", "Advanced", 100)]
    [ConventionTemplate]
    public class GalleryItem : AbstractContentPage
    {
        public GalleryItem()
        {
            Visible = false;
        }

        [FileAttachment, EditableFileUploadAttribute("Image", 30, ContainerName = Tabs.Content)]
        public virtual string ImageUrl
        {
            get { return (string)base.GetDetail("ImageUrl"); }
            set { base.SetDetail("ImageUrl", value); }
        }

        public virtual string ResizedImageUrl
        {
            get { return Web.Adapters.ImageAdapter.GetResizedImageUrl(ImageUrl, Gallery.MaxImageWidth, Gallery.MaxImageHeight); }
        }

        public virtual string ThumbnailImageUrl
        {
            get { return Web.Adapters.ImageAdapter.GetResizedImageUrl(ImageUrl, Gallery.MaxThumbnailWidth, Gallery.MaxThumbnailHeight); }
        }

        public virtual ImageGallery Gallery
        {
            get { return Parent as ImageGallery; }
        }

        public override string Url
        {
            get { return N2.Web.Url.Parse(Parent.Url).AppendQuery(PathData.ItemQueryKey, ID).SetFragment("#t" + ID); }
        }
    }
}
