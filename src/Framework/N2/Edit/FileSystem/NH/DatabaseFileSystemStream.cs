using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NHibernate;

namespace N2.Edit.FileSystem.NH
{
    /// <summary>
    /// Allows reading and writing to files in the database.
    /// </summary>
    internal class DatabaseFileSystemStream : Stream
    {
        private readonly FileSystemItem file;
        private readonly DatabaseFileSystem fs;
        private ISession session;
        private int chunkSize;

        public DatabaseFileSystemStream(FileSystemItem file, DatabaseFileSystem fileSys, ISession session, int chunkSize)
        {
            if (file == null) throw new Exception("attachedTo must not be null");
            if (fileSys == null) throw new Exception("fileSys most not be null");

            this.file = file;
            this.fs = fileSys;
            this.session = session;
            this.chunkSize = chunkSize;

            Position = 0;
        }

        public override sealed long Position { get; set; }

        public override long Length
        {
            get { return file.Length ?? 0; }
        }

        MemoryStream writeStream;
        MemoryStream WriteStream
        {
            get { return writeStream ?? (writeStream = new MemoryStream()); }
        }

        public override sealed void Write(byte[] buffer, int offset, int count)
        {
            WriteStream.Write(buffer, offset, count);
        }

        public override void WriteByte(byte value)
        {
            WriteStream.WriteByte(value);
        }

        public override void Flush()
        {
            if(writeStream != null)
                fs.UpdateFile(file.Path, writeStream);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            //  000000000011111111112222222233333333
            //  0-9       10-19     20-29
            //  10        10        10

            //  0-7     8-15    16-23   24-31
            //  8       8       8       8

            int writtenBytes = 0;

            var pos = (int)Position + offset;
            foreach(var chunk in session.CreateQuery("from " + typeof(FileSystemChunk).Name + " fsc where fsc.BelongsTo = :file and fsc.Offset >= :minimum and fsc.Offset < :maximum order by fsc.Offset")
                .SetParameter("file", file)
                .SetParameter("minimum", pos - chunkSize)
                .SetParameter("maximum", pos + count)
                .Enumerable<FileSystemChunk>())
            {
                if (chunk.Offset >= pos)
                {
                    int bytes = Math.Min(chunk.Data.Length, count - (chunk.Offset - pos));
                    writtenBytes += bytes;
                    Array.Copy(chunk.Data, 0, buffer, chunk.Offset - pos, bytes);
                }
                else //chunk.Offset < offset
                {
                    int bytes = Math.Min(chunk.Data.Length - (pos - chunk.Offset), count);
                    writtenBytes += bytes;
                    Array.Copy(chunk.Data, pos - chunk.Offset, buffer, 0, bytes);
                }
            }

            Position += writtenBytes;
            return writtenBytes;
        }

        public override int ReadByte()
        {
            var buffer = new byte[1];
            if(Read(buffer, (int)Position, 1) <= 0)
                return -1;
            return buffer[0];
        }

        protected override void Dispose(bool disposing)
        {
            Flush();
            base.Dispose(disposing);
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            if (origin == SeekOrigin.Begin)
                Position = offset;
            else if (origin == SeekOrigin.Current)
                Position += offset;
            else
                Position = file.Length ?? 0 + offset;
            
            return Position;
        }

        public override void SetLength(long value)
        {
        }
    }
}
