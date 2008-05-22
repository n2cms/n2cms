using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace N2.Templates.Imaging
{
	public class ImageResizer
	{
		public void Resize(string imagePath, double width, double height, Stream output)
		{
			if (imagePath == null) throw new ArgumentNullException("imagePath");

			if (File.Exists(imagePath))
			{
				using (Bitmap original = new Bitmap(imagePath))
				{
					double ratioY = height / original.Height;
					double ratioX = width / original.Width;
					double ratio = Math.Min(ratioX, ratioY);
					if (ratio <= 0)
						ratio = 1;

					int newWidth = (int)(original.Width * ratio);
					int newHeight = (int)(original.Height * ratio);

					Bitmap resized = new Bitmap(newWidth, newHeight, original.PixelFormat);
					resized.SetResolution(original.HorizontalResolution, original.VerticalResolution);

					Graphics g;
					switch (Path.GetExtension(imagePath).ToLower())
					{
						case ".jpg":
						case ".jpeg":
							g = Graphics.FromImage(resized);
							break;
						default:
							Bitmap temp = new Bitmap(resized.Width, resized.Height);
							g = Graphics.FromImage(temp);
							g.DrawImage(resized, new Rectangle(0, 0, temp.Width, temp.Height), 0, 0, resized.Width, resized.Height, GraphicsUnit.Pixel);
							resized = temp;
							break;
					}

					using (g)
					{
						g.PageUnit = GraphicsUnit.Pixel;
						g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
						g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
						g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
						g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
						g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

						using (ImageAttributes attr = new ImageAttributes())
						{
							attr.SetWrapMode(WrapMode.TileFlipXY);
							g.DrawImage(original, new Rectangle(0, 0, newWidth, newHeight), 0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attr);
						}
						resized.Save(output, ImageFormat.Jpeg);
					}
				}
			}
		}


		public void Test()
		{
			//string imagePath = Page.Server.MapPath(ImageControl.ImageUrl);
			//string thumbnailUrl = string.Format("~/Temp/{0}_{1}_{2}{3}",
			//        Path.GetFileNameWithoutExtension(imagePath),
			//        ImageControl.Width.Value,
			//        ImageControl.Height.Value,
			//        Path.GetExtension(imagePath));
			//string thumbnailPath = Page.Server.MapPath(thumbnailUrl);

			//if (!File.Exists(thumbnailPath))
			//    GenerateThumbnailImage(imagePath, thumbnailPath);
		}
	}
}
