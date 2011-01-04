using N2.Definitions;
using N2.Details;
using N2.Integrity;
using N2.Web.UI;

namespace N2.Templates.Mvc.Models.Pages
{
	[PageDefinition("Image Gallery", Description = "Displays an image with next/previous thumbnails", SortOrder = 220,
		IconUrl = "~/Content/Img/photos.png")]
	[RestrictParents(typeof (IStructuralPage))]
	[FieldSetContainer(ImageGallery.GallerySettings, "Gallery Settings", 500, ContainerName = Tabs.Content)]
	public class ImageGallery : ContentPageBase
	{
		#region GallerySettings

		public const string GallerySettings = "gallerySettings";

		[EditableImageSize("Preferred Image Size", 200, ContainerName = GallerySettings)]
		public virtual string PreferredImageSize
		{
			get { return GetDetail("PreferredImageSize", "original"); }
			set { SetDetail("PreferredImageSize", value, "original"); }
		}

		[EditableImageSize("Preferred Thumbnail Size", 202, ContainerName = GallerySettings)]
		public virtual string PreferredThumbnailSize
		{
			get { return GetDetail("PreferredThumbnailSize", "thumb"); }
			set { SetDetail("PreferredThumbnailSize", value, "thumb"); }
		}

		#endregion
	}
}