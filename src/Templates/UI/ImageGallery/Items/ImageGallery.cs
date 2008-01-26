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
using N2.Templates.Items;

namespace N2.Templates.ImageGallery.Items
{
	[Definition("Image Gallery", "ImageGallery", "Displays an image with next/previous thumbnails", "", 220)]
	[RestrictParents(typeof(IStructuralPage))]
	public class ImageGallery : AbstractContentPage
	{
		[EditableTextBox("Max Image Width", 200, ContainerName = "advanced")]
		public virtual int MaxImageWidth
		{
			get { return (int)(GetDetail("MaxImageWidth") ?? 490); }
			set { SetDetail("MaxImageWidth", value); }
		}

		[EditableTextBox("Max Image Height", 210, ContainerName = "advanced")]
		public virtual int MaxImageHeight
		{
			get { return (int)(GetDetail("MaxImageHeight") ?? 490); }
			set { SetDetail("MaxImageHeight", value); }
		}

		[EditableTextBox("Max Thumbnail Width", 220, ContainerName = "advanced")]
		public virtual int MaxThumbnailWidth
		{
			get { return (int)(GetDetail("MaxThumbnailWidth") ?? 70); }
			set { SetDetail("MaxThumbnailWidth", value); }
		}

		[EditableTextBox("Max Thumbnail Height", 230, ContainerName = "advanced")]
		public virtual int MaxThumbnailHeight
		{
			get { return (int)(GetDetail("MaxThumbnailHeight") ?? 55); }
			set { SetDetail("MaxThumbnailHeight", value); }
		}

		public override string IconUrl
		{
			get
			{
				return "~/ImageGallery/UI/Img/Photos.png";
			}
		}

		public override string TemplateUrl
		{
			get
			{
				return "~/ImageGallery/UI/Default.aspx";
			}
		}
	}
}
