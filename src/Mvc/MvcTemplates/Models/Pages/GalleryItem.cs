using N2.Details;
using N2.Edit.FileSystem;
using N2.Integrity;
using N2.Persistence.Serialization;
using N2.Web;
using N2.Web.Drawing;
using N2.Web.UI;

namespace N2.Templates.Mvc.Models.Pages
{
	[PageDefinition("Gallery Item",
		IconUrl = "~/Content/Img/photo.png")]
	[RestrictParents(typeof (ImageGallery))]
	[TabContainer("advanced", "Advanced", 100)]
	public class GalleryItem : ContentPageBase
	{
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

		public virtual string GetResizedImageUrl(IFileSystem fs)
		{
			return GetReizedUrl(fs, Gallery.PreferredImageSize);
		}

		public virtual string GetThumbnailImageUrl(IFileSystem fs)
		{
			return GetReizedUrl(fs, Gallery.PreferredThumbnailSize);
		}

		private string GetReizedUrl(IFileSystem fs, string imageSize)
		{
			string resizedUrl = ImagesUtility.GetResizedPath(ImageUrl, imageSize);
			if (fs.FileExists(resizedUrl))
				return resizedUrl;
			return ImageUrl;
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