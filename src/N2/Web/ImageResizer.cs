using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using N2.Engine;

namespace N2.Web
{
	/// <summary>
	/// Resizes an image.
	/// </summary>
	[Service]
    public class ImageResizer
    {
        private void Resize(string imagePath, double width, double height, Stream output)
        {
            if (imagePath == null) throw new ArgumentNullException("imagePath");

            if (File.Exists(imagePath))
            {
                using (Bitmap original = new Bitmap(imagePath))
				{
					double ratio = GetRatio(original, width, height);
					Resize(Path.GetExtension(imagePath), original, ratio, output);
                }
            }
		}

		public virtual void Resize(Stream inputStream, string extension, double newWidth, double newHeight, Stream outputStream)
		{
			using (Bitmap original = new Bitmap(inputStream))
			{
				double ratio = GetRatio(original, newWidth, newHeight);
				Resize(extension, original, ratio, outputStream);
			}
		}

		double GetRatio(Bitmap original, double width, double height)
		{
			double ratioY = height / original.Height;
			double ratioX = width / original.Width;
			
			double ratio = Math.Min(ratioX, ratioY);
			if(ratio == 0)
				ratio = Math.Max(ratioX, ratioY);
			if (ratio <= 0 || ratio > 1) ratio = 1;
			return ratio;
		}

    	void Resize(string extension, Bitmap original, double ratio, Stream output)
    	{
    		int newWidth = (int)Math.Round(original.Width * ratio);
    		int newHeight = (int)Math.Round(original.Height * ratio);

    		Bitmap resized = new Bitmap(newWidth, newHeight, original.PixelFormat);
    		resized.SetResolution(original.HorizontalResolution, original.VerticalResolution);

    		Graphics g;
			switch (extension.ToLower())
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
    			g.InterpolationMode = InterpolationMode.HighQualityBicubic;
    			g.SmoothingMode = SmoothingMode.HighQuality;
    			g.PixelOffsetMode = PixelOffsetMode.HighQuality;
    			g.CompositingMode = CompositingMode.SourceCopy;
    			g.CompositingQuality = CompositingQuality.HighQuality;

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