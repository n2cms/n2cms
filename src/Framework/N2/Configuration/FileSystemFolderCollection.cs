using System.Collections.Generic;
using System.Configuration;

namespace N2.Configuration
{
    [ConfigurationCollection(typeof(FolderElement))]
    public class FileSystemFolderCollection : LazyRemovableCollection<FolderElement>
    {
        public void Add(string folderPath)
        {
            BaseAdd(new FolderElement { Path = folderPath });
        }

        public FolderElement this[int index]
        {
            get { return (FolderElement)base.BaseGet(index); }
        }

        public IEnumerable<string> Folders
        {
            get
            {
                foreach(FolderElement element in this.AllElements)
                {
                    if (element.Path.EndsWith("/"))
                        yield return element.Path;
                    else
                        yield return element.Path + "/";
                }
            }
        }

        protected override string ElementKeyAttributeName
        {
            get
            {
                return "path";
            }
        }
    }
}
