using System;
using System.IO;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls.Adapters;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace N2.Templates.Web.Adapters
{
	public class ImageAdapter : WebControlAdapter
	{
		protected System.Web.UI.WebControls.Image ImageControl
		{
			get { return base.Control as System.Web.UI.WebControls.Image; }
		}

		protected override void Render(HtmlTextWriter writer)
		{
			if (!ImageControl.Height.IsEmpty || !ImageControl.Width.IsEmpty)
			{
				string thumbnailUrl = GetResizedImageUrl(ImageControl.ImageUrl, ImageControl.Height.Value, ImageControl.Width.Value);

				writer.WriteBeginTag("img");
				writer.WriteAttribute("alt", ImageControl.AlternateText);
				writer.WriteAttribute("src", thumbnailUrl);
				writer.WriteEndTag("img");
			}
			else if (ImageControl.ImageUrl.Length > 0)
				base.Render(writer);
		}

		public static string GetResizedImageUrl(string imageUrl, double width, double height)
		{
			return string.Format("{0}?w={1}&amp;h={2}&amp;img={3}",
								VirtualPathUtility.ToAbsolute("~/Templates/UI/Image.ashx"),
								width,
								height,
								HttpUtility.UrlEncode(imageUrl));
		}
	}
}
