using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BuildSupport
{
	public partial class Commands
	{
		public void ZipDirectory(string zipFilePath, string directoryPathToCompress, string directoryPathInArchive)
		{
			Ionic.Zip.ZipFile zf = new Ionic.Zip.ZipFile(zipFilePath);
			zf.CompressionLevel = Ionic.Zlib.CompressionLevel.None;
			zf.AddDirectory(directoryPathToCompress, directoryPathInArchive);
			zf.AddProgress += delegate(object sender, Ionic.Zip.AddProgressEventArgs e)
			{
				Console.WriteLine("\tAdding " + e.CurrentEntry.FileName);
			};
			zf.Save();
		}
	}
}
