using System;
using System.Configuration;
using System.Web;
using System.Web.UI.WebControls;
using N2.Web;
using N2.Web.Drawing;

namespace N2.Edit.Web.UI.Controls
{
    public class ResizedImage : Image
    {
        static Url ImageHandlerUrl = N2.Context.Current.ManagementPaths.ResolveResourceUrl("{ManagementUrl}/Files/Resize.ashx");

		public int MaxWidth { get; set; }
        public int MaxHeight { get; set; }
        public string Hash { get; set; }
		public string DetailImageResizeUrl { get; set; }


		public override void RenderBeginTag(System.Web.UI.HtmlTextWriter writer)
        {
			Url url;
			if (!String.IsNullOrEmpty(DetailImageResizeUrl))
			{
				url = GetCustomResizedImageUrl(DetailImageResizeUrl, ImageUrl);
				//hash is making problem with s3 do not include it
			}
			else
			{
				url = GetResizedImageUrl(ImageUrl, MaxWidth, MaxHeight);
				if (!string.IsNullOrEmpty(Hash))
					url = url.AppendQuery("hash", Hash);
			}
            writer.AddAttribute("src", url);
            base.RenderBeginTag(writer);
        }

        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            if (string.IsNullOrEmpty(ImageUrl))
                return;
            if (!ImagesUtility.IsImagePath(ImageUrl))
                return;

            base.Render(writer);
        }

        /// <summary>Returns the path to an image handler that resizes the given image to the appropriate size.</summary>
        /// <param name="imageUrl">The image to resize.</param>
        /// <param name="width">The maximum width.</param>
        /// <param name="height">The maximum height.</param>
        /// <returns>The path to a handler that performs resizing of the image.</returns>
        public static string GetResizedImageUrl(string imageUrl, double width, double height)
        {
            return GetResizedImageUrl(imageUrl, width, height, ImageResizeMode.Fit);
        }

        /// <summary>Returns the path to an image handler that resizes the given image to the appropriate size.</summary>
        /// <param name="imageUrl">The image to resize.</param>
        /// <param name="width">The maximum width.</param>
        /// <param name="height">The maximum height.</param>
        /// <param name="mode">How to fit the image</param>
        /// <returns>The path to a handler that performs resizing of the image.</returns>
        public static string GetResizedImageUrl(string imageUrl, double width, double height, ImageResizeMode mode)
        {
            string fileExtension = VirtualPathUtility.GetExtension(Url.PathPart(imageUrl));

            bool isAlreadyImageHandler = string.Equals(fileExtension, ".ashx", StringComparison.OrdinalIgnoreCase);
            if (isAlreadyImageHandler) return Url.ToAbsolute(imageUrl);

            Url url = ImageHandlerUrl.SetQueryParameter("img", N2.Context.Current.ManagementPaths.ResolveResourceUrl(imageUrl));
            if (width > 0) url = url.SetQueryParameter("w", (int)width);
            if (height > 0) url = url.SetQueryParameter("h", (int)height);
            url = url.AppendQuery("m", mode.ToString());
            return url;
        }

		/// <summary>
		/// Fill custom url for resizing with image name and extension
		/// </summary>
		/// <param name="customResizeUrl">Custom Url with {0} for image url, and {1} for image extension</param>
		/// <param name="imageUrl">The image to resize</param>
		/// <returns></returns>
		public static string GetCustomResizedImageUrl(string customResizeUrl, string imageUrl)
		{
			string imageExtension = VirtualPathUtility.GetExtension(Url.PathPart(imageUrl));
			string imageUrlWithoutExtension = imageUrl.Replace(imageExtension, string.Empty);

			return string.Format(customResizeUrl, imageUrlWithoutExtension, imageExtension);
		}
	}
}
