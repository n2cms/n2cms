using N2.Configuration;
using N2.Engine;
using N2.Web.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Management.Files.FileSystem.Pages
{
    [Service]
    public class ImageSizeCache
    {
        public HashSet<string> ImageSizes { get; private set; }

        public ImageSizeCache(ConfigurationManagerWrapper config)
        {
            ImageSizes = new HashSet<string>(config.Sections.Management.Images.Sizes.AllElements.Select(ise => ise.Name), StringComparer.InvariantCultureIgnoreCase);
        }

        public virtual string GetSizeName(string fileName)
        {
            int separatorIndex;
            int dotIndex;
            if (!TryGetSeparatorAndDotIndex(fileName, out separatorIndex, out dotIndex))
                return null;

            var sizeName = fileName.Substring(separatorIndex + 1, dotIndex - separatorIndex - 1);
            if (!ImageSizes.Contains(sizeName))
                return null;

            return sizeName;
        }

        public virtual string RemoveImageSize(string fileName)
        {
            int separatorIndex;
            int dotIndex;
            if (!TryGetSeparatorAndDotIndex(fileName, out separatorIndex, out dotIndex))
                return null;

            return fileName.Substring(0, separatorIndex) + fileName.Substring(dotIndex);
        }

        private static bool TryGetSeparatorAndDotIndex(string fileName, out int separatorIndex, out int dotIndex)
        {
            separatorIndex = fileName.LastIndexOf(ImagesUtility.Separator[0]);
            dotIndex = fileName.LastIndexOf('.');

            return separatorIndex >= 0 && separatorIndex < dotIndex;
        }
    }
}
