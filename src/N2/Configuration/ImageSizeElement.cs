using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

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
	}
}
