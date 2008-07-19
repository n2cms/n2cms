using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace N2.Configuration
{
    public class FileSystemFolderCollection : ConfigurationElementCollection
    {
        public FileSystemFolderCollection()
        {
            FolderElement folder = new FolderElement();
            folder.Path = "~/Upload";
            base.BaseAdd(folder);
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new FolderElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((FolderElement)element).Path;
        }

        public FolderElement this[int index]
        {
            get { return (FolderElement)base.BaseGet(index); }
        }
    }
}
