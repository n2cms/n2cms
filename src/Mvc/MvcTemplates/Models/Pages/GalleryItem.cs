using N2.Details;
using N2.Edit.FileSystem;
using N2.Integrity;
using N2.Persistence.Serialization;
using N2.Web;
using N2.Web.Drawing;
using N2.Web.UI;
using N2.Engine;

namespace N2.Templates.Mvc.Models.Pages
{
    [PageDefinition("Gallery Item",
        IconClass = "fa fa-picture")]
    [RestrictParents(typeof (ImageGallery))]
    [TabContainer("advanced", "Advanced", 100)]
    public class GalleryItem : ContentPageBase, IInjectable<IFileSystem>
    {
        private IFileSystem fs;

        public IFileSystem Fs
        {
            get { return fs ?? (fs = Context.Current.Resolve<IFileSystem>()); }
            set { fs = value; }
        }

        public GalleryItem()
        {
            Visible = false;
        }

        [FileAttachment, EditableFileUploadAttribute("Image", 30, ContainerName = Tabs.Content)]
        public virtual string ImageUrl
        {
            get { return (string) base.GetDetail("ImageUrl"); }
            set { base.SetDetail("ImageUrl", value); }
        }

        public virtual string GetResizedImageUrl()
        {
            return ImagesUtility.GetExistingImagePath(Fs, ImageUrl, "wide");
        }

        public virtual string GetThumbnailImageUrl()
        {
            return ImagesUtility.GetExistingImagePath(Fs, ImageUrl, "thumb");
        }

        public virtual ImageGallery Gallery
        {
            get { return Parent as ImageGallery; }
        }

        public override string Url
        {
            get { return N2.Web.Url.Parse(Parent.Url).AppendQuery(PathData.ItemQueryKey, ID).SetFragment("#t" + ID); }
        }


        public void Set(IFileSystem dependency)
        {
            fs = dependency;
        }
    }
}
