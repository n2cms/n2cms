using System.Linq;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using N2.Web.Drawing;

namespace N2.Configuration
{
    public class ImagesElement : ConfigurationElement
    {
        [ConfigurationProperty("resizeUploadedImages", DefaultValue = true)]
        public bool ResizeUploadedImages
        {
            get { return (bool)base["resizeUploadedImages"]; }
            set { base["resizeUploadedImages"] = value; }
        }

        [ConfigurationProperty("resizedExtensions", DefaultValue = ".jpg,.jpeg"), TypeConverter(typeof(CommaDelimitedStringCollectionConverter))]
        public StringCollection ResizedExtensions
        {
            get { return (CommaDelimitedStringCollection)base["resizedExtensions"]; }
            set { base["resizedExtensions"] = value; }
        }

		[ConfigurationProperty("useCustomResizing", DefaultValue = false)]
		public bool UseCustomResizing
		{
			get { return (bool)base["useCustomResizing"]; }
			set { base["useCustomResizing"] = value; }
		}

		[ConfigurationProperty("resizeSeparator", DefaultValue = "-__-")]
        public string ResizeSeparator
        {
            get { return (string)base["resizeSeparator"]; }
            set { base["resizeSeparator"] = value; }
        }

		[ConfigurationProperty("customImagePath", DefaultValue = "")]
		public string CustomImagePath
		{
			get { return (string)base["customImagePath"]; }
			set { base["customImagePath"] = value; }
		}

		[ConfigurationProperty("customThumbResizePattern", DefaultValue = "")]
		public string CustomThumbResizePattern
		{
			get { return (string)base["customThumbResizePattern"]; }
			set { base["customThumbResizePattern"] = value; }
		}

		[ConfigurationProperty("customDetailResizePattern", DefaultValue = "")]
		public string CustomDetailResizePattern
		{
			get { return (string)base["customDetailResizePattern"]; }
			set { base["customDetailResizePattern"] = value; }
		}

		/// <summary>Information about images.</summary>
		[ConfigurationProperty("sizes")]
        public ImageSizesCollection Sizes
        {
            get { return (ImageSizesCollection)base["sizes"]; }
            set { base["sizes"] = value; }
        }

        public ImageSizeElement GetImageSize(string imageUrl)
        {
            string baseImagePath;
            string imageSize;
            ImagesUtility.SplitImageAndSize(imageUrl, Sizes.GetSizeNames(), out baseImagePath, out imageSize);
            return Sizes.FirstOrDefault(s => s.Name == imageSize);
        }
    }
}
