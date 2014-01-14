using System.Configuration;
using System.Linq;
using N2.Web.Drawing;
using System.Collections.Generic;

namespace N2.Configuration
{
    [ConfigurationCollection(typeof(ImageSizeElement))]
    public class ImageSizesCollection : LazyRemovableCollection<ImageSizeElement>
    {
        public ImageSizesCollection()
        {
            AddDefault(new ImageSizeElement { Name = "", Description = "Default", Width = 500, Height = 500, Replace = true });
            AddDefault(new ImageSizeElement { Name = "icon", Description = "Icon", Width = 16, Height = 16, Mode = ImageResizeMode.Fill, Announced = false });
            AddDefault(new ImageSizeElement { Name = "original", Description = "Original", Width = 0, Height = 0 });
        }

        public IEnumerable<string> GetSizeNames()
        {
            return AllElements.Select(e => e.Name);
        }
    }
}
