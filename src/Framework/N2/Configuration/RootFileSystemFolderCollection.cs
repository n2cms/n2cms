using System.Collections.Generic;
using System.Configuration;

namespace N2.Configuration
{
    [ConfigurationCollection(typeof(FolderElement))]
    public class RootFileSystemFolderCollection : FileSystemFolderCollection
    {
        public RootFileSystemFolderCollection()
        {
            FolderElement folder = new FolderElement();
            folder.Path = "~/upload/";
            AddDefault(folder);
        }
    }
}
