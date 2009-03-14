using N2.Integrity;
using N2.Details;
using N2.Web.UI;
using N2.Serialization;

namespace N2.Templates.Items
{
    [Definition("Gallery Item", "GalleryItem")]
    [RestrictParents(typeof(ImageGallery))]
    [TabContainer("advanced", "Advanced", 100)]
	[DefaultTemplate("GalleryItem")]
	public class GalleryItem : AbstractContentPage
    {
        public GalleryItem()
        {
            Visible = false;
        }

        [EditableImage("Image", 30, ContainerName = Tabs.Content)]
        [FileAttachment]
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
			get { return N2.Web.Url.Parse(Parent.Url).AppendQuery("item", ID).SetFragment("#t" + ID); }
        }

        protected override string IconName
        {
            get { return "photo"; }
        }
    }
}