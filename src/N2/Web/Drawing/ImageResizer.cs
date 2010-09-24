using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using N2.Engine;
using N2.Configuration;

namespace N2.Web.Drawing
{
	/// <summary>
	/// Resizes an image.
	/// </summary>
	[Service]
    public class ImageResizer
    {
		[Obsolete("Use overload with resize mode")]
		private void Resize(string imagePath, double maxWidth, double maxHeight, Stream output)
		{
			Resize(imagePath, maxWidth, maxHeight, ImageResizeMode.Fit, output);
		}

		private bool Resize(string physicalImagePath, double maxWidth, double maxHeight, ImageResizeMode mode, Stream outputStream)
        {
            if (physicalImagePath == null) throw new ArgumentNullException("imagePath");

			if (!File.Exists(physicalImagePath))
				return false;
            
            using (Bitmap original = new Bitmap(physicalImagePath))
			{
				//if (original.Width < maxWidth && original.Height < maxHeight)
				//{
				//    using (var fs = File.OpenRead(physicalImagePath))
				//    {
				//        TransferBetweenStreams(fs, outputStream);
				//    }
				//    return true;
				//}
				Resize(Path.GetExtension(physicalImagePath), original, maxWidth, maxHeight, mode, outputStream);
				return true;
            }
		}

		[Obsolete("Use overload with resize mode")]
		public virtual void Resize(Stream inputStream, string extension, double maxWidth, double maxHeight, Stream outputStream)
		{
			Resize(inputStream, extension, maxWidth, maxHeight, ImageResizeMode.Fit, outputStream);
		}

		public virtual bool Resize(Stream inputStream, string extension, double maxWidth, double maxHeight, ImageResizeMode mode, Stream outputStream)
		{
			using (Bitmap original = new Bitmap(inputStream))
			{
				//if (original.Width < maxWidth && original.Height < maxHeight)
				//{
				//    TransferBetweenStreams(inputStream, outputStream);
				//    return true;
				//}

				Resize(extension, original, maxWidth, maxHeight, mode, outputStream);
				return true;
			}
		}

    	private void Resize(string extension, Bitmap original, double maxWidth, double maxHeight, ImageResizeMode mode, Stream output)
    	{
    		Bitmap resized;

			if (mode == ImageResizeMode.Fit)
			{
				double resizeRation = GetResizeRatio(original, maxWidth, maxHeight);
				int newWidth = (int)Math.Round(original.Width * resizeRation);
				int newHeight = (int)Math.Round(original.Height * resizeRation);
				resized = new Bitmap(newWidth, newHeight, original.PixelFormat);
			}
			else
				resized = new Bitmap((int)maxWidth, (int)maxHeight, original.PixelFormat);

    		resized.SetResolution(original.HorizontalResolution, original.VerticalResolution);

    		Graphics g;
			switch (extension.ToLower())
    		{
    			case ".jpg":
    			case ".jpeg":
    				g = Graphics.FromImage(resized);
    				break;
    			default:
    				Bitmap temp = new Bitmap(resized.Width, resized.Height, original.PixelFormat);
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

					Rectangle destinationFrame = mode == ImageResizeMode.Fill
						? GetFillDestinationRectangle(original.Size, resized.Size)
						: new Rectangle(Point.Empty, resized.Size);
					g.DrawImage(original, destinationFrame, 0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attr);
				}
    			resized.Save(output, original.RawFormat);
    		}
    	}

		public static Rectangle GetFillDestinationRectangle(Size original, Size resized)
		{
			double resizeWidth = resized.Width / (double)original.Width;
			double resizeHeight = resized.Height / (double)original.Height;
			double resizeMax = Math.Max(resizeWidth, resizeHeight);

			var size = new Size((int)(original.Width * resizeMax), (int)(original.Height * resizeMax));

			bool isBeeingMadeNarrower = resizeWidth < resizeHeight;

			Point relocated = isBeeingMadeNarrower
				? new Point((resized.Width - size.Width) / 2, 0)
				: new Point(0, (resized.Height - size.Height) / 2);

			return new Rectangle(relocated, size);
		}

		double GetResizeRatio(Bitmap original, double width, double height)
		{
			double ratioY = height / original.Height;
			double ratioX = width / original.Width;

			double ratio = Math.Min(ratioX, ratioY);
			if (ratio == 0)
				ratio = Math.Max(ratioX, ratioY);
			if (ratio <= 0 || ratio > 1) ratio = 1;
			return ratio;
		}

		protected virtual void TransferBetweenStreams(Stream inputStream, Stream outputStream)
		{
			byte[] buffer = new byte[32768];
			while (true)
			{
				int bytesRead = inputStream.Read(buffer, 0, buffer.Length);
				if (bytesRead <= 0)
					break;

				outputStream.Write(buffer, 0, bytesRead);
			}
		}
    }
}