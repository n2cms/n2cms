using System;
using System.Collections.Generic;

namespace N2.Edit.FileSystem.NH
{

	public class FileSystemChunk
	{
		public virtual int ID { get; protected set; }
		public virtual FileSystemItem BelongsTo { get; set; }
		public virtual int Offset { get; set; }
		public virtual byte[] Data { get; set; }
	}

	/// <summary>
	/// A persisted file or folder in the file system.
	/// </summary>
    public class FileSystemItem
    {
        public virtual int ID { get; protected set; }
        public virtual FileSystemPath Path { get; set; }
		public virtual DateTime Created { get; set; }
		public virtual DateTime Updated { get; set; }
		public virtual long? Length { get; set; }
		public virtual ICollection<FileSystemChunk> Chunks { get; set; }

        public virtual FileData ToFileData()
        {
            return new FileData
            {
                Created = Created,
                Updated = Updated,
                VirtualPath = Path.ToString(),
                Name = Path.Name,
                Length = Length.Value
            };
        }

        public virtual DirectoryData ToDirectoryData()
        {
            return new DirectoryData
            {
                Created = Created,
                Updated = Updated,
                VirtualPath = Path.ToString(),
                Name = Path.Name
            };
        }

	}
}
