using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using N2.Engine;
using N2.Configuration;

namespace N2.Web.Drawing
{
	public class ImageResizeParameters
	{
		public ImageResizeParameters(double maxWidth, double maxHeight)
			: this(maxWidth, maxHeight, ImageResizeMode.Fit)
		{
		}

		public ImageResizeParameters(double maxWidth, double maxHeight, ImageResizeMode mode)
		{
			Mode = mode;
			MaxWidth = maxWidth;
			MaxHeight = maxHeight;
			Quality = 90L;
		}

		public ImageResizeMode Mode { get; set; }
		public double MaxWidth { get; set; }
		public double MaxHeight { get; set; }
		public long Quality { get; set; }
	}

	/// <summary>
	/// Resizes an image.
	/// </summary>
	[Service]
    public class ImageResizer
    {
		[Obsolete("Use overload with parameters")]
		private void Resize(string imagePath, double maxWidth, double maxHeight, Stream output)
		{
			Resize(imagePath, maxWidth, maxHeight, ImageResizeMode.Fit, output);
		}

		[Obsolete("Use overload with parameters")]
		public virtual void Resize(Stream inputStream, string extension, double maxWidth, double maxHeight, Stream outputStream)
		{
			Resize(inputStream, new ImageResizeParameters(maxWidth, maxHeight, ImageResizeMode.Fit), outputStream);
		}

		[Obsolete("Use overload with parameters")]
		public virtual bool Resize(Stream inputStream, string extension, double maxWidth, double maxHeight, ImageResizeMode mode, Stream outputStream)
		{
			return Resize(inputStream, new ImageResizeParameters(maxWidth, maxHeight, mode), outputStream);
		}

		[Obsolete("Use overload with parameters")]
		public virtual byte[] GetResizedBytes(Stream imageStream, string extension, double maxWidth, double maxHeight, ImageResizeMode mode)
		{
			return GetResizedBytes(imageStream, new ImageResizeParameters(maxWidth, maxHeight, mode));
		}



		public virtual bool Resize(Stream inputStream, ImageResizeParameters parameters, Stream outputStream)
		{
			using (Bitmap original = new Bitmap(inputStream))
			{
				Resize(original, parameters, outputStream);
				return true;
			}
		}

		public virtual byte[] GetResizedBytes(Stream imageStream, ImageResizeParameters parameters)
		{
			using (Bitmap original = new Bitmap(imageStream))
			{
				var ms = new MemoryStream();
				Resize(original, parameters, ms);
				return ms.GetBuffer();
			}
		}



		private bool Resize(string physicalImagePath, double maxWidth, double maxHeight, ImageResizeMode mode, Stream outputStream)
		{
			if (physicalImagePath == null) throw new ArgumentNullException("imagePath");

			if (!File.Exists(physicalImagePath))
				return false;

			using (Bitmap original = new Bitmap(physicalImagePath))
			{
				Resize(original, new ImageResizeParameters(maxWidth, maxHeight, mode), outputStream);
				return true;
			}
		}

		private void Resize(Bitmap original, ImageResizeParameters parameters, Stream output)
    	{
    		Bitmap resized;
			var mode = parameters.Mode;
			var maxWidth = parameters.MaxWidth;
			var maxHeight = parameters.MaxHeight;
			var quality = parameters.Quality;

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

    		Graphics g = CreateGraphics(original, ref resized);

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

                // Use higher quality compression if the original image is jpg. Default is 75L.
                ImageCodecInfo codec = GetEncoderInfo(original.RawFormat.Guid);

                if (codec != null && codec.MimeType.Equals("image/jpeg"))
                {
                    EncoderParameters encoderParams = new EncoderParameters(1);
                    encoderParams.Param[0] = new EncoderParameter(Encoder.Quality, parameters.Quality);

                    resized.Save(output, codec, encoderParams);                    
                }
                else
                {
    			    resized.Save(output, original.RawFormat);
                }
    		}
    	}

        private ImageCodecInfo GetEncoderInfo(Guid formatID)
        {
            // Get image codecs for all image formats
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();

            // Find the correct image codec
            for (int i = 0; i < codecs.Length; i++)
                if (codecs[i].FormatID == formatID)
                    return codecs[i];
            return null;
        }

		private Graphics CreateGraphics(Bitmap original, ref Bitmap resized)
		{
			ImageCodecInfo info = GetEncoderInfo(original.RawFormat.Guid);

			switch (info.MimeType)
			{
				case "image/jpeg":
					if (resized.PixelFormat == PixelFormat.Format1bppIndexed || resized.PixelFormat == PixelFormat.Format4bppIndexed || resized.PixelFormat == PixelFormat.Format8bppIndexed)
						return GetResizedBitmap(ref resized, PixelFormat.Format24bppRgb);
					return Graphics.FromImage(resized);
				case "image/gif":
					return GetResizedBitmap(ref resized, PixelFormat.Format24bppRgb);
				case "image/png":
				default:
					return GetResizedBitmap(ref resized, original.PixelFormat);
			}
		}

		private Graphics GetResizedBitmap(ref Bitmap resized, PixelFormat format)
		{
			Bitmap temp = new Bitmap(resized.Width, resized.Height, format);
			var g = Graphics.FromImage(temp);
			g.DrawImage(resized, new Rectangle(0, 0, temp.Width, temp.Height), 0, 0, resized.Width, resized.Height, GraphicsUnit.Pixel);
			resized = temp;
			return g;
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