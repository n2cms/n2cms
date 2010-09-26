using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using N2.Web.Drawing;

namespace N2.Configuration
{
	public class ImageSizeElement : NamedElement
	{
		/// <summary>Maximum width of images resized to this size.</summary>
		[ConfigurationProperty("width", DefaultValue = 0)]
		public int Width
		{
			get { return (int)base["width"]; }
			set { base["width"] = value; }
		}

		/// <summary>Maximum height of images resized to this size.</summary>
		[ConfigurationProperty("height", DefaultValue = 0)]
		public int Height
		{
			get { return (int)base["height"]; }
			set { base["height"] = value; }
		}

		/// <summary>Replace existing file when creating this image size.</summary>
		[ConfigurationProperty("replace", DefaultValue = false)]
		public bool Replace
		{
			get { return (bool)base["replace"]; }
			set { base["replace"] = value; }
		}

		/// <summary>Replace existing file when creating this image size.</summary>
		[ConfigurationProperty("resizeOnUpload", DefaultValue = true)]
		public bool ResizeOnUpload
		{
			get { return (bool)base["resizeOnUpload"]; }
			set { base["resizeOnUpload"] = value; }
		}

		/// <summary>Maximum height of images resized to this size.</summary>
		[ConfigurationProperty("mode", DefaultValue = ImageResizeMode.Fit)]
		public ImageResizeMode Mode
		{
			get { return (ImageResizeMode)base["mode"]; }
			set { base["mode"] = value; }
		}
	}
}
