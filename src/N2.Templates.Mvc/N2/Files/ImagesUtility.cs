using System;
using System.Collections.Generic;
using System.Web;
using N2.Web;

namespace Management.N2.Files
{
	public class ImagesUtility
	{
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

		public static string GetResizedPath(Url url, string resizedSuffix)
		{
			return url.PathWithoutExtension + Separator + resizedSuffix + url.Extension;
		}

		public const string Separator = "_";
	}
}
