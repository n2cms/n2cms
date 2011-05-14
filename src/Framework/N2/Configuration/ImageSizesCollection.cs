using System.Configuration;
using N2.Web.Drawing;

namespace N2.Configuration
{
	[ConfigurationCollection(typeof(ImageSizeElement))]
	public class ImageSizesCollection : LazyRemovableCollection<ImageSizeElement>
	{
		public ImageSizesCollection()
		{
			AddDefault(new ImageSizeElement { Name = "", Width = 500, Height = 500, Replace = true });
			AddDefault(new ImageSizeElement { Name = "icon", Width = 16, Height = 16, Mode = ImageResizeMode.Fill });
			AddDefault(new ImageSizeElement { Name = "original", Width = 0, Height = 0 });
		}
	}
}
