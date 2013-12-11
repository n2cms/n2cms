using System;
using System.Web;
using System.Web.Hosting;
using N2.Edit.FileSystem;
using System.Collections.Generic;

namespace N2.Web.Drawing
{
    /// <summary>
    /// Helper methods for images and image paths.
    /// </summary>
    public static class ImagesUtility
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
            return Context.Current.Resolve<IFileSystem>().GetExistingImagePath(imageUrl, preferredSize);
        }

        /// <summary>Gets the path of a resized version of the image, if it exists, otherwise the given image url.</summary>
        /// <param name="url">The base url of an image.</param>
        /// <param name="resizedSuffix">The size suffix to append before the extension.</param>
        /// <returns>A resized image url if it exists.</returns>
        public static string GetExistingImagePath(string imageUrl, string preferredSize, out bool preferredSizeExists)
        {
            return Context.Current.Resolve<IFileSystem>().GetExistingImagePath(imageUrl, preferredSize, out preferredSizeExists);
        }

        /// <summary>Gets the path of a resized version of the image, if it exists, otherwise the given image url.</summary>
        /// <param name="fs">The file system used to check file existence.</param>
        /// <param name="url">The base url of an image.</param>
        /// <param name="resizedSuffix">The size suffix to append before the extension.</param>
        /// <returns>A resized image url if it exists.</returns>
        public static string GetExistingImagePath(this IFileSystem fs, string imageUrl, string preferredSize)
        {
            bool preferredSizeExists;
            return GetExistingImagePath(fs, imageUrl, preferredSize, out preferredSizeExists);
        }

        /// <summary>Gets the path of a resized version of the image, if it exists, otherwise the given image url.</summary>
        /// <param name="fs">The file system used to check file existence.</param>
        /// <param name="url">The base url of an image.</param>
        /// <param name="resizedSuffix">The size suffix to append before the extension.</param>
        /// <returns>A resized image url if it exists.</returns>
        public static string GetExistingImagePath(this IFileSystem fs, string imageUrl, string preferredSize, out bool preferredSizeExists)
        {
            if (string.IsNullOrEmpty(imageUrl) || string.IsNullOrEmpty(preferredSize))
            {
                preferredSizeExists = false;
                return imageUrl;
            }

            string preferredUrl = ImagesUtility.GetResizedPath(imageUrl, preferredSize);
            try
            {
                if (fs.FileExists(preferredUrl))
                {
                    preferredSizeExists = true;
                    return preferredUrl;
                }
            }
            catch (InvalidOperationException)
            {
            }

            preferredSizeExists = false;
            return Url.ToAbsolute(imageUrl);
        }

        public static string GetIconUrl(string resourceUrl)
        {
            return IconPath(GetExtensionGroup(VirtualPathUtility.GetExtension(resourceUrl)));
        }

        public static string GetExtensionGroup(string extension)
        {
            return ExtensionGroupSelector.GetExtensionGroup(extension);
        }

        private static string IconPath(string iconName)
        {
            return N2.Web.Url.ResolveTokens("{ManagementUrl}/Resources/icons/" + iconName + ".png");
        }

        #region ExtensionGroups
        private static ExtensionGroups extensionGroupSelector = new ExtensionGroups();
        public static ExtensionGroups ExtensionGroupSelector
        {
            get { return ImagesUtility.extensionGroupSelector; }
            set { ImagesUtility.extensionGroupSelector = value; }
        }
        public class ExtensionGroups
        {
            public const string Images = "page_white_picture";
            public const string Pdf = "page_white_acrobat";
            public const string ServerCode = "page_white_csharp";
            public const string ClientCode = "page_white_code";
            public const string Compressed = "page_white_compressed";
            public const string Flash = "page_white_flash";
            public const string Text = "page_white_text";
            public const string Excel = "page_white_excel";
            public const string Powerpoint = "page_white_powerpoint";
            public const string Word = "page_white_word";
            public const string Audio = "page_white_cd";
            public const string Video = "page_white_dvd";
            public const string Default = "page_white";
            public virtual string GetExtensionGroup(string extension)
            {
                extension = (extension ?? "").ToLower();
                switch (extension)
                {
                    case ".gif":
                    case ".png":
                    case ".jpg":
                    case ".tif":
                    case ".tiff":
                    case ".jpeg":
                        return ExtensionGroups.Images;
                    case ".pdf":
                        return ExtensionGroups.Pdf;
                    case ".c":
                    case ".cpp":
                    case ".class":
                    case ".java":
                    case ".cs":
                    case ".vb":
                    case ".js":
                    case ".dtd":
                    case ".m":
                    case ".pl":
                    case ".py":
                        return ExtensionGroups.ServerCode;
                    case ".html":
                    case ".htm":
                    case ".xml":
                    case ".xsd":
                    case ".xslt":
                    case ".aspx":
                    case ".ascx":
                    case ".ashx":
                    case ".php":
                    case ".css":
                    case ".sql":
                    case ".fla":
                        return ExtensionGroups.ClientCode;
                    case ".zip":
                    case ".gz":
                    case ".7z":
                    case ".rar":
                    case ".sit":
                    case ".tar":
                    case ".pkg":
                    case ".msi":
                    case ".cab":
                        return ExtensionGroups.Compressed;
                    case ".swf":
                        return ExtensionGroups.Flash;
                    case ".txt":
                    case ".log":
                        return ExtensionGroups.Text;
                    case ".csv":
                    case ".xls":
                    case ".xlsx":
                    case ".wks":
                        return ExtensionGroups.Excel;
                    case ".xps":
                    case ".ppt":
                    case ".pptx":
                    case ".pps":
                        return ExtensionGroups.Powerpoint;
                    case ".rtf":
                    case ".doc":
                    case ".docx":
                        return ExtensionGroups.Word;
                    case ".mpg":
                    case ".mpeg":
                    case ".avi":
                    case ".wmv":
                    case ".flv":
                    case ".mp4":
                    case ".mov":

                    case ".3g2":
                    case ".3gp":
                    case ".asf":
                    case ".asx":
                    case ".rm":
                    case ".vob":
                        return ExtensionGroups.Video;

                    case ".aif":
                    case ".iff":
                    case ".m3u":
                    case ".m4a":
                    case ".mid":
                    case ".mp3":
                    case ".mpa":
                    case ".ra":
                    case ".wav":
                    case ".wma":
                        return ExtensionGroups.Audio;
                    default:
                        return ExtensionGroups.Default;
                }
            }
        }
        #endregion

        public static string GetSize(string imageUrl, IEnumerable<string> possibleSizes)
        {
            string path;
            string size;
            SplitImageAndSize(imageUrl, possibleSizes, out path, out size);
            return size;
        }

        public static void SplitImageAndSize(string imageUrl, IEnumerable<string> possibleSizes, out string baseImagePath, out string imageSize)
        {
            var extension = Url.Parse(imageUrl).Extension;
            foreach (var size in possibleSizes)
            {
                string suffix = Separator + size + extension;
                if (imageUrl.EndsWith(suffix))
                {
                    baseImagePath = imageUrl.Substring(0, imageUrl.Length - suffix.Length) + extension;
                    imageSize = size;
                    return;
                }
            }
            baseImagePath = imageUrl;
            imageSize = "";
        }
    }
}
