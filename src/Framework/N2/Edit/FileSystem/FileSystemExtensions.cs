using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Edit.FileSystem
{
	public static class FileSystemExtensions
	{
		public static DirectoryData GetDirectoryOrVirtual(this IFileSystem fs, string virtualDir)
		{
			return fs.GetDirectory(virtualDir)
				?? DirectoryData.Virtual(virtualDir);
		}
	}
}
