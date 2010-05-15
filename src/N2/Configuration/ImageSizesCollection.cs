using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace N2.Configuration
{
	[ConfigurationCollection(typeof(ImageSizeElement))]
	public class ImageSizesCollection : ConfigurationElementCollection
	{
		public ImageSizesCollection()
		{
			BaseAdd(new ImageSizeElement { Name = "", Width = 500, Height = 500, Replace = true });
			BaseAdd(new ImageSizeElement { Name = "icon", Width = 32, Height = 16 });
			//BaseAdd(new ImageSizeElement { Name = "thumbnail", Width = 64, Height = 64 });
			BaseAdd(new ImageSizeElement { Name = "original", Width = 0, Height = 0 });
		}

		protected override ConfigurationElement CreateNewElement()
		{
			return new ImageSizeElement();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((ImageSizeElement)element).Name;
		}
	}
}
