using System.Collections.Generic;
using System.Configuration;

namespace N2.Configuration
{
	[ConfigurationCollection(typeof(FolderElement))]
	public class FileSystemFolderCollection : ConfigurationElementCollection
    {
        public FileSystemFolderCollection()
        {
            FolderElement folder = new FolderElement();
            folder.Path = "~/upload/";
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
    			foreach(FolderElement element in this)
    			{
					if (element.Path.EndsWith("/"))
						yield return element.Path;
					else
						yield return element.Path + "/";
    			}
    		}
    	}
    }
}
