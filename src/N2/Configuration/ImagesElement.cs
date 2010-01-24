using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

		/// <summary>Information about images.</summary>
		[ConfigurationProperty("sizes")]
		public ImageSizesCollection Sizes
		{
			get { return (ImageSizesCollection)base["sizes"]; }
			set { base["sizes"] = value; }
		}
	}
}
