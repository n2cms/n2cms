using System;

namespace N2.Edit.FileSystem
{
	public class FileData
	{
		public string Name { get; set; }
		public string VirtualPath { get; set; }
		public long Length { get; set; }
		public DateTime Created { get; set; }
		public DateTime Updated { get; set; }
	}
}