using N2.Integrity;
using N2.Details;
using N2.Web.UI;
using N2.Serialization;

namespace N2.Templates.Items
{
    [Definition("Gallery Item", "GalleryItem")]
    [RestrictParents(typeof(ImageGallery))]
    [TabPanel("advanced", "Advanced", 100)]
    public class GalleryItem : AbstractPage
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

        [EditableFreeTextArea("Text", 40, ContainerName = Tabs.Content)]
        public virtual string Text
        {
            get { return (string)(GetDetail("Text") ?? string.Empty); }
            set { SetDetail("Text", value, string.Empty); }
        }

        [EditableCheckBox("Visible", 40, ContainerName = Tabs.Advanced)]
        public override bool Visible
        {
            get { return base.Visible; }
            set { base.Visible = value; }
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
            get { return Parent.Url + "?item=" + ID+ "#t" + ID; }
        }

        protected override string IconName
        {
            get { return "photo"; }
        }

        protected override string TemplateName
        {
            get { return "GalleryItem"; }
        }
    }
}