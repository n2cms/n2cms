using System;
using System.Web;
using System.Web.Hosting;

namespace N2.Web.Drawing
{
	/// <summary>
	/// Helper methods for images and image paths.
	/// </summary>
	public class ImagesUtility
	{
		/// <summary>Compares the extension against known image exensions.</summary>
		/// <param name="imageUrl">The url to check</param>
		/// <returns>True if it's an image</returns>
		public static bool IsImagePath(string imageUrl)
		{
			string fileExtension = VirtualPathUtility.GetExtension(Url.PathPart(imageUrl));
			switch (fileExtension.ToLower())
			{
				case ".gif":
				case ".jpg":
				case ".jpeg":
				case ".png":
				case ".tiff":
				case ".tif":
					return true;
			}
			return false;
		}

		/// <summary>Gets the path of a resized version of the image, regardless of it existing or not.</summary>
		/// <param name="url">The base url of an image.</param>
		/// <param name="resizedSuffix">The size suffix to append before the extension.</param>
		/// <returns>A resized image url.</returns>
		public static string GetResizedPath(Url url, string resizedSuffix)
		{
			if(!string.IsNullOrEmpty(resizedSuffix))
				return url.PathWithoutExtension + Separator + resizedSuffix + url.Extension;
			return url.Path;
		}

		/// <summary>Separator between the size suffix and the file name.</summary>
		public const string Separator = "_";

		/// <summary>Gets the path of a resized version of the image, if it exists, otherwise the given image url.</summary>
		/// <param name="url">The base url of an image.</param>
		/// <param name="resizedSuffix">The size suffix to append before the extension.</param>
		/// <returns>A resized image url if it exists.</returns>
		public static string GetExistingImagePath(string imageUrl, string preferredSize)
		{
			if (string.IsNullOrEmpty(imageUrl) || string.IsNullOrEmpty(preferredSize))
				return imageUrl;

			string preferredUrl = ImagesUtility.GetResizedPath(imageUrl, preferredSize);
			try
			{
				if (HostingEnvironment.VirtualPathProvider.FileExists(preferredUrl))
					return preferredUrl;
			}
			catch (InvalidOperationException)
			{
			}

			return Url.ToAbsolute(imageUrl);
		}
	}
}
