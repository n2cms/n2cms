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
	internal class DatabaseFileSystemStream : MemoryStream
	{
		private readonly FileSystemItem file;
		private readonly DatabaseFileSystem fs;
		private bool changed;
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

		public override sealed long Position
		{
			get { return base.Position; }
			set { base.Position = value; }
		}

		public override long Length
		{
			get { return Math.Max(base.Length, file.Length.Value); }
		}

		public override sealed void Write(byte[] buffer, int offset, int count)
		{
			changed = true;
			base.Write(buffer, offset, count);
		}

		public override void WriteByte(byte value)
		{
			changed = true;
			base.WriteByte(value);
		}

		public override void Flush()
		{
			if (changed)
				fs.UpdateFile(file.Path, this);

			base.Flush();
		}

		object endedBuffer = null;
		public override int Read(byte[] buffer, int offset, int count)
		{
			if (object.ReferenceEquals(endedBuffer, buffer))
				return 0;

			//  000000000011111111112222222233333333
			//	0-9       10-19     20-29
			//  10        10        10

			//  0-7     8-15    16-23   24-31
			//  8       8       8       8

			int writtenBytes = 0;

			foreach(var chunk in session.CreateQuery("from " + typeof(FileSystemChunk).Name + " fsc where fsc.BelongsTo = :file and fsc.Offset >= :minimum and fsc.Offset < :maximum order by fsc.Offset")
				.SetParameter("file", file)
				.SetParameter("minimum", offset - chunkSize)
				.SetParameter("maximum", offset + count)
				.Enumerable<FileSystemChunk>())
			{
				if (chunk.Offset >= offset)
				{
					int bytes = Math.Min(chunk.Data.Length, count - (chunk.Offset - offset));
					writtenBytes += bytes;
					Array.Copy(chunk.Data, 0, buffer, chunk.Offset - offset, bytes);
				}
				else //chunk.Offset < offset
				{
					int bytes = Math.Min(chunk.Data.Length - (offset - chunk.Offset), count);
					writtenBytes += bytes;
					Array.Copy(chunk.Data, offset - chunk.Offset, buffer, 0, bytes);
				}
			}

			if (writtenBytes == Length)
				endedBuffer = buffer;

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
	}
}
