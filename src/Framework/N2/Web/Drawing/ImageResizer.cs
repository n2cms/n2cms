using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using N2.Engine;

namespace N2.Web.Drawing
{
    /// <summary>
    /// Parameters for the <see cref="ImageResizer"/>.
    /// </summary>
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
            Quality = 90;
        }

        public ImageResizeMode Mode { get; set; }
        public double MaxWidth { get; set; }
        public double MaxHeight { get; set; }
        public int Quality { get; set; }

        public Rectangle? SourceRectangle { get; set; }

        public static ImageResizeParameters Fill(Rectangle sourceRectangle, int maxWidth, int maxHeight)
        {
            return new ImageResizeParameters(maxWidth, maxHeight, ImageResizeMode.Fill) { SourceRectangle = sourceRectangle };
        }
    }

    /// <summary>
    /// Resizes an image.
    /// </summary>
    [Service]
    public class ImageResizer
    {
		Logger<ImageResizer> log;

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
            if (inputStream == null) throw new ArgumentNullException("inputStream");

            using (Bitmap original = new Bitmap(inputStream))
            {
				try
				{
					Resize(original, parameters, outputStream);
					return true;
				}
				catch (Exception ex)
				{
					log.Error(ex);
					throw;
				}
            }
        }

        public virtual byte[] GetResizedBytes(Stream imageStream, ImageResizeParameters parameters)
        {
            if (imageStream == null) throw new ArgumentNullException("imageStream");

            using (Bitmap original = new Bitmap(imageStream))
            {
                var ms = new MemoryStream();
                Resize(original, parameters, ms);
                return ms.GetBuffer();
            }
        }

        private void Resize(Bitmap original, ImageResizeParameters parameters, Stream output)
        {
            Bitmap resized = null;
            try
            {
                var mode = parameters.Mode;
                var maxWidth = parameters.MaxWidth;
                var maxHeight = parameters.MaxHeight;
                var quality = parameters.Quality;
                var srcRect = parameters.SourceRectangle;
                Rectangle dest;
                double resizeRatio;
                int newWidth, newHeight;

                switch (mode)
                {
                    case ImageResizeMode.Fit:
                        resizeRatio = GetResizeRatio(original, maxWidth, maxHeight);
                        newWidth = (int)Math.Round(original.Width * resizeRatio);
                        newHeight = (int)Math.Round(original.Height * resizeRatio);
                        resized = new Bitmap(newWidth, newHeight, original.PixelFormat);
                        dest = new Rectangle(Point.Empty, resized.Size);
                        break;
                    case ImageResizeMode.FitCenterOnTransparent:
                        resizeRatio = GetResizeRatio(original, maxWidth, maxHeight);
                        newWidth = (int)Math.Round(original.Width * resizeRatio);
                        newHeight = (int)Math.Round(original.Height * resizeRatio);
                        int newImageX = (maxWidth < maxHeight) ? 0 : ((int)((maxWidth - (original.Width * resizeRatio)) / 2));
                        int newImageY = newImageX != 0 ? 0 : ((int)((maxHeight - (original.Height * resizeRatio)) / 2));
                        resized = new Bitmap((int)maxWidth, (int)maxHeight, PixelFormat.Format32bppArgb);
                        dest = new Rectangle(newImageX, newImageY, newWidth, newHeight);
                        break;
                    default:
                        resized = new Bitmap((int)maxWidth, (int)maxHeight, original.PixelFormat);
                        dest = new Rectangle(Point.Empty, resized.Size);
                        break;
                }


                resized.SetResolution(original.HorizontalResolution, original.VerticalResolution);

                using (Graphics g = CreateGraphics(original, ref resized))
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
                        var originalSize = srcRect.HasValue ? srcRect.Value.Size : original.Size;

                        if (mode == ImageResizeMode.Fill)
                            dest = GetFillDestinationRectangle(originalSize, resized.Size);

                        var src = new Rectangle(0, 0, original.Width, original.Height);
                        if (srcRect.HasValue)
                            src = srcRect.Value;

                        if (mode == ImageResizeMode.FitCenterOnTransparent)
                            g.Clear(Color.Transparent);

                        g.DrawImage(original, dest, src.X, src.Y, src.Width, src.Height, GraphicsUnit.Pixel, attr);
                    }

                    // Use higher quality compression if the original image is jpg. Default is 75L.
                    var codec = GetEncoderInfo(original.RawFormat.Guid);
                    if (codec != null && codec.MimeType.Equals("image/jpeg"))
                    {
                        EncoderParameters encoderParams = new EncoderParameters(1);
                        encoderParams.Param[0] = new EncoderParameter(Encoder.Quality, (long)quality);
                        resized.Save(output, codec, encoderParams);
                    }
                    else
                    {
                        resized.Save(output, original.RawFormat);
                    }
                }
            }
            finally
            {
                if (resized != null)
                    resized.Dispose();
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
                    if (resized.PixelFormat == PixelFormat.Format1bppIndexed || resized.PixelFormat == PixelFormat.Format4bppIndexed || resized.PixelFormat == PixelFormat.Format8bppIndexed)
                        return GetResizedBitmap(ref resized, PixelFormat.Format24bppRgb);
                    return Graphics.FromImage(resized);
                default:
                    return GetResizedBitmap(ref resized, original.PixelFormat);
            }
        }

        private Graphics GetResizedBitmap(ref Bitmap resized, PixelFormat format)
        {
            if (format == PixelFormat.Indexed)
                format = PixelFormat.Format24bppRgb;
            Bitmap temp = new Bitmap(resized.Width, resized.Height, format);
            var g = Graphics.FromImage(temp);
            g.DrawImage(resized, new Rectangle(0, 0, temp.Width, temp.Height), 0, 0, resized.Width, resized.Height, GraphicsUnit.Pixel);
            resized = temp;
            return g;
        }

        public static Rectangle GetFillDestinationRectangle(Size original, Size resized)
        {
            var resizeWidth = resized.Width / (double)original.Width;
            var resizeHeight = resized.Height / (double)original.Height;
            var resizeMax = Math.Max(resizeWidth, resizeHeight);
            var size = new Size((int)(original.Width * resizeMax), (int)(original.Height * resizeMax));
            var isBeeingMadeNarrower = resizeWidth < resizeHeight;
            var relocated = isBeeingMadeNarrower
                ? new Point((resized.Width - size.Width) / 2, 0)
                : new Point(0, (resized.Height - size.Height) / 2);

            return new Rectangle(relocated, size);
        }

        static double GetResizeRatio(Bitmap original, double width, double height)
        {
            double ratioY = height / original.Height;
            double ratioX = width / original.Width;
            double ratio = Math.Min(ratioX, ratioY);
            if (Math.Abs(ratio - 0.0) < double.Epsilon)
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
