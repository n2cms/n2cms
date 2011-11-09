using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Management.Files
{
	internal struct FolderPair
	{
		public FolderPair(int parentID, string parentPath, string path, string folderPath)
		{
			ParentID = parentID;
			Path = path;
			ParentPath = parentPath;
			FolderPath = folderPath;
		}

		public int ParentID;
		public string Path;
		public string ParentPath;
		public string FolderPath;
	}
}
