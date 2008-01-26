using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using N2.Integrity;
using N2.Details;

namespace N2.Templates.ImageGallery.Items
{
	[Definition("Gallery Item", "GalleryItem")]
	[RestrictParents(typeof(ImageGallery))]
	[WithEditableTitle("Title", 10)]
	public class GalleryItem : Templates.Items.AbstractItem
	{
		[EditableImage("Image", 30)]
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
			get { return this.Parent as ImageGallery; }
		}

		public override string IconUrl
		{
			get
			{
				return "~/ImageGallery/UI/Img/Photo.png";
			}
		}

		public override string TemplateUrl
		{
			get
			{
				return "~/ImageGallery/UI/GalleryItem.ascx";
			}
		}
	}
}
