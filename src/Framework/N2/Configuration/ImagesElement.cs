using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;

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

		/// <summary>Information about images.</summary>
		[ConfigurationProperty("sizes")]
		public ImageSizesCollection Sizes
		{
			get { return (ImageSizesCollection)base["sizes"]; }
			set { base["sizes"] = value; }
		}
	}
}
