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

		[ConfigurationProperty("description")]
		public string Description
		{
			get { return (string)base["description"]; }
			set { base["description"] = value; }
		}

		/// <summary>Maximum height of images resized to this size.</summary>
		[ConfigurationProperty("height", DefaultValue = 0)]
		public int Height
		{
			get { return (int)base["height"]; }
			set { base["height"] = value; }
		}

		/// <summary>Maximum height of images resized to this size.</summary>
		[ConfigurationProperty("quality", DefaultValue = 90)]
		public int Quality
		{
			get { return (int)base["quality"]; }
			set { base["quality"] = value; }
		}

		/// <summary>Replace existing file when creating this image size.</summary>
		[ConfigurationProperty("replace", DefaultValue = false)]
		public bool Replace
		{
			get { return (bool)base["replace"]; }
			set { base["replace"] = value; }
		}

		/// <summary>Whether this size should be announced in the management interface..</summary>
		[ConfigurationProperty("announced", DefaultValue = true)]
		public bool Announced
		{
			get { return (bool)base["announced"]; }
			set { base["announced"] = value; }
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
