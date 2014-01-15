using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Edit.FileSystem.NH
{
    /// <summary>
    /// A part of a file's data stored in the database file system.
    /// </summary>
    public class FileSystemChunk
    {
        public virtual int ID { get; protected set; }
        public virtual FileSystemItem BelongsTo { get; set; }
        public virtual int Offset { get; set; }
        public virtual byte[] Data { get; set; }
    }
}
