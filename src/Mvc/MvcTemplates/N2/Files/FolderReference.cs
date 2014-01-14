using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Edit;

namespace N2.Management.Files
{
    public struct FolderReference
    {
        public FolderReference(int parentID, string parentPath, string path, FileSystemRoot folder)
        {
            ParentID = parentID;
            Path = path;
            ParentPath = parentPath;
            Folder = folder;
        }

        public int ParentID;
        public string Path;
        public string ParentPath;
        public FileSystemRoot Folder;
    }
}
