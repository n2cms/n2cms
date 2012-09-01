using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using N2.Configuration;
using N2.Persistence.NH;
using N2.Plugin;
using NHibernate.Criterion;
using N2.Web;
using N2.Engine;
using System.Diagnostics;
using NHibernate;

namespace N2.Edit.FileSystem.NH
{
	/// <summary>
	/// A file system implementation that stores files in the database.
	/// </summary>
	[Service(typeof(IFileSystem), Configuration = "dbfs", Replaces = typeof(MappedFileSystem))]
	public class DatabaseFileSystem : IFileSystem
	{
		private readonly Engine.Logger<DatabaseFileSystem> logger;
		private readonly ISessionProvider _sessionProvider;
		private const long UploadFileSize = long.MaxValue;
		private int chunkSize;

		public DatabaseFileSystem(ISessionProvider sessionProvider, DatabaseSection config)
        {
            this._sessionProvider = sessionProvider;
			this.chunkSize = config.Files.ChunkSize;
        }

        private IEnumerable<FileSystemItem> FindChildren(FileSystemPath path, ICriterion criterion)
        {
			return Session.CreateCriteria<FileSystemItem>()
                .Add(Restrictions.Eq("Path.Parent", path.ToString()))
                .Add(criterion)
				.AddOrder(Order.Asc("Path.Name"))
                .List<FileSystemItem>();
        }

        private FileSystemItem GetSpecificItem(FileSystemPath path)
        {
            var c = Session.CreateCriteria<FileSystemItem>()
                .Add(Restrictions.Eq("Path", path));

            return c.UniqueResult<FileSystemItem>();
        }

        private void EnsureParentExists(FileSystemPath target)
        {
            if(!DirectoryExists(target.Parent))
            {
                CreateDirectoryInternal(target.Parent);
            }
        }

        private void AssertParentExists(FileSystemPath target)
        {
            EnsureParentExists(target);

            if (!DirectoryExists(target.Parent))
            {
                throw new DirectoryNotFoundException("Destination directory not found: " + target.Parent);
            }
        }

        public IEnumerable<FileData> GetFiles(string parentVirtualPath)
        {
            var path = FileSystemPath.Directory(parentVirtualPath);

            return from child in FindChildren(path, Restrictions.IsNotNull("Length"))
                   select child.ToFileData();
        }

        public FileData GetFile(string virtualPath)
        {
            var path = FileSystemPath.File(virtualPath);
			var item = GetSpecificItem(path);

			return item == null
				? null
				: item.ToFileData();
        }

        public IEnumerable<DirectoryData> GetDirectories(string parentVirtualPath)
        {
            var path = FileSystemPath.Directory(parentVirtualPath);

            return from child in FindChildren(path, Restrictions.IsNull("Length"))
                   select child.ToDirectoryData();
        }

        public DirectoryData GetDirectory(string virtualPath)
        {
            var path = FileSystemPath.Directory(virtualPath);
            var item = GetSpecificItem(path);

            return item == null
                ? null
                : item.ToDirectoryData();
        }

        public bool FileExists(string virtualPath)
        {
            var path = FileSystemPath.File(virtualPath);
            var item = GetSpecificItem(path);
            return item != null && item.Length.HasValue;
        }

        public void MoveFile(string fromVirtualPath, string destinationVirtualPath)
        {
            var source = FileSystemPath.File(fromVirtualPath);
            var target = FileSystemPath.File(destinationVirtualPath);

            using (var trx = Session.BeginTransaction())
            {
                AssertParentExists(target);

                var file = GetSpecificItem(source);
                file.Path = target;
				file.Updated = Utility.CurrentTime();
                trx.Commit();
            }

            if (FileMoved != null)
            {
                FileMoved.Invoke(this, new FileEventArgs(destinationVirtualPath, fromVirtualPath));
            }
        }

        public void DeleteFile(string virtualPath)
        {
            using (var trx = Session.BeginTransaction())
            {
                var path = FileSystemPath.File(virtualPath);

				var file = GetSpecificItem(path);

				int deletedChunks = Session.CreateQuery("delete from " + typeof(FileSystemChunk).Name + " where BelongsTo = :file")
					.SetParameter("file", file)
					.ExecuteUpdate();

				Session.Delete(file);
				logger.Debug("Deleted 1 item and " + deletedChunks + " chunks at " + path);

                trx.Commit();
            }

            if (FileDeleted != null)
            {
                FileDeleted.Invoke(this, new FileEventArgs(virtualPath, null));
            }
        }

        public void CopyFile(string fromVirtualPath, string destinationVirtualPath)
        {
            var source = FileSystemPath.File(fromVirtualPath);
            var target = FileSystemPath.File(destinationVirtualPath);

			var s = Session;
            using (var trx = s.BeginTransaction())
            {
                AssertParentExists(target);

                var file = GetSpecificItem(source);

                var copy = new FileSystemItem
                {
                    Path = target,
                    Length = file.Length,
					Created = Utility.CurrentTime(),
					Updated = Utility.CurrentTime()
                };

                s.Save(copy);

				foreach (var sourceChunk in GetChunks(file))
				{
					var chunkCopy = CreateChunk(copy, sourceChunk.Offset, sourceChunk.Data);
					s.Save(chunkCopy);
					
					s.Flush();
					s.Evict(sourceChunk);
					s.Evict(chunkCopy);
				}

                trx.Commit();
            }

            if (FileCopied != null)
            {
                FileCopied.Invoke(this, new FileEventArgs(destinationVirtualPath, fromVirtualPath));
            }
        }

		private NHibernate.ISession Session
		{
			get { return _sessionProvider.OpenSession.Session; }
		}

		public Stream OpenFile(string virtualPath, bool readOnly = false)
        {
            var path = FileSystemPath.File(virtualPath);

            if(!FileExists(path.ToString()))
            {
                CreateFile(path, new MemoryStream());
            }
            var blob = GetSpecificItem(path);
			var stream = new DatabaseFileSystemStream(blob, this, Session, chunkSize);
			return stream;
        }

        public void WriteFile(string virtualPath, Stream inputStream)
        {
            if (!FileExists(virtualPath))
            {
                CreateFile(FileSystemPath.File(virtualPath), inputStream);
            }
            else
            {
                UpdateFile(FileSystemPath.File(virtualPath), inputStream);
            }

            if (FileWritten != null)
            {
                FileWritten.Invoke(this, new FileEventArgs(virtualPath, null));
            }
        }

        private static bool IsStreamSizeOk(Stream inputStream)
        {
            if (inputStream == null || !inputStream.CanSeek)
                return true;

            return UploadFileSize > inputStream.Length;
        }

        private static void CheckStreamSize(Stream inputStream, FileSystemPath virtualPath)
        {
            if (!IsStreamSizeOk(inputStream))
                throw new Exception(virtualPath + " could not be uploaded created because its size (" + inputStream.Length + ") exceeds the configured UploadFileMaxSize");
        }

        internal void UpdateFile(FileSystemPath virtualPath, Stream inputStream)
        {
            CheckStreamSize(inputStream, virtualPath);
			if (inputStream.CanSeek && inputStream.Position > 0)
				inputStream.Position = 0;

            var file = GetSpecificItem(virtualPath);
			file.Updated = Utility.CurrentTime();

			var s = Session;
			using (var trx = s.BeginTransaction())
			{
				var temp = new byte[chunkSize];
				int offset = 0;
				int length = 0;

				bool endOfInputFile = false;
				foreach (var chunk in GetChunks(file.Path))
				{
					if (endOfInputFile)
					{
						s.Delete(chunk);
						s.Flush();
						s.Evict(chunk);
						continue;
					}

					int size = inputStream.Read(temp, 0, temp.Length);
					endOfInputFile = size <= 0;
					length += size;
					
					if (endOfInputFile)
					{
						s.Delete(chunk);
						s.Flush();
						s.Evict(chunk);
						continue;
					}

					var buffer = Copy(temp, size);

					chunk.Offset = offset;
					chunk.Data = buffer;
					s.Update(chunk);
					s.Flush();
					s.Evict(chunk);

					offset += size;
				}

				while (true && !endOfInputFile)
				{
					int size = inputStream.Read(temp, 0, temp.Length);
					if (size <= 0)
						break;

					length += size;
					var buffer = Copy(temp, size);
					var chunk = CreateChunk(file, offset, buffer);
					s.Save(chunk);
					s.Flush();
					s.Evict(chunk);
				}

				file.Length = length;
				s.Update(file);
				
				trx.Commit();
			}
        }

		private static byte[] Copy(byte[] temp, int size)
		{
			var buffer = new byte[size];
			Array.Copy(temp, buffer, size);
			return buffer;
		}

        private void CreateFile(FileSystemPath virtualPath, Stream inputStream)
        {
			EnsureParentExists(virtualPath);

            CheckStreamSize(inputStream, virtualPath);
			long fileSize = inputStream.CanSeek ? inputStream.Length : 0;
			
			using (var trx = Session.BeginTransaction())
			{
				var item = new FileSystemItem
				{
					Path = virtualPath,
					Created = Utility.CurrentTime(),
					Updated = Utility.CurrentTime(),
					Length = fileSize
				};
				Session.Save(item);

				var temp = new byte[chunkSize];
				int offset = 0;
				int size = 0;
				while(true)
				{
					size = inputStream.Read(temp, 0, temp.Length);
					if (size <= 0)
						break;

					var buffer = Copy(temp, size);

					var chunk = CreateChunk(item, offset, buffer);
					Session.Save(chunk);

					if (size < temp.Length)
						break;

					offset += size;
				}

				if (fileSize == 0)
				{
					item.Length = offset + size;
					Session.Update(item);
				}

				trx.Commit();
			}
			//using (var buffer = (inputStream.CanSeek && (inputStream.Length <= int.MaxValue) ? new MemoryStream((int)inputStream.Length) : new MemoryStream()))
			//{
			//    AssertParentExists(virtualPath);

			//    inputStream.Position = 0;
			//    CopyStream(inputStream, buffer);
			//    inputStream.Position = 0;

			//    var item = new FileSystemItem
			//                   {
			//                       Path = virtualPath,
			//                       Data = (inputStream.CanSeek && inputStream.Length == buffer.GetBuffer().Length ) ? buffer.GetBuffer() : buffer.ToArray(),
			//                       Created = Utility.CurrentTime(),
			//                       Updated = Utility.CurrentTime(),
			//                       Length = buffer.Length
			//                   };

			//    Session.Save(item);
			//    trx.Commit();
			//}
        }

		private static FileSystemChunk CreateChunk(FileSystemItem item, int offset, byte[] buffer)
		{
			var chunk = new FileSystemChunk
			{
				BelongsTo = item,
				Data = buffer,
				Offset = offset
			};
			return chunk;
		}

        public void ReadFileContents(string virtualPath, Stream outputStream)
        {
            var path = FileSystemPath.File(virtualPath);
			foreach (var chunk in GetChunks(path))
			{
				outputStream.Write(chunk.Data, 0, chunk.Data.Length);
				Session.Evict(chunk);
			}
        }

		private IEnumerable<FileSystemChunk> GetChunks(FileSystemPath filePath)
		{
			return Session.CreateQuery("from FileSystemChunk fsc where fsc.BelongsTo.Path.Parent = :parentPath and fsc.BelongsTo.Path.Name = :name order by fsc.Offset")
				.SetParameter("parentPath", filePath.Parent)
				.SetParameter("name", filePath.Name)
				.Enumerable<FileSystemChunk>();
		}

		private IEnumerable<FileSystemChunk> GetChunks(FileSystemItem file)
		{
			return Session.CreateQuery("from FileSystemChunk fsc where fsc.BelongsTo = :belongsTo order by fsc.Offset")
				.SetParameter("belongsTo", file)
				.Enumerable<FileSystemChunk>();
		}

        public bool DirectoryExists(string virtualPath)
        {
            var path = FileSystemPath.Directory(virtualPath);
            var item = GetSpecificItem(path);
            return item != null;
        }

        public void MoveDirectory(string fromVirtualPath, string destinationVirtualPath)
        {
            var source = FileSystemPath.Directory(fromVirtualPath);
            var target = FileSystemPath.Directory(destinationVirtualPath);

            if (target.IsDescendantOf(source))
            {
                throw new ApplicationException("Cannot move directory into own subdictory.");
            }

			using (var trx = Session.BeginTransaction())
            {
                var directory = GetSpecificItem(source);
				var descendants = Session
                    .CreateCriteria<FileSystemItem>()
                    .Add(Restrictions.Like("Path.Parent", directory.Path.ToString(), MatchMode.Start))
                    .List<FileSystemItem>();

                foreach (var item in descendants)
                {
                    item.Path.Rebase(source, target);
                }

				directory.Updated = Utility.CurrentTime();
                directory.Path = target;
				Session.Update(directory);

                trx.Commit();
            }

            if (DirectoryMoved != null)
            {
                DirectoryMoved.Invoke(this, new FileEventArgs(destinationVirtualPath, fromVirtualPath));
            }
        }

        public void DeleteDirectory(string virtualPath)
        {
            var path = FileSystemPath.Directory(virtualPath);

			using (var trx = Session.BeginTransaction())
            {
				DeleteDescendants(path);

				var directory = GetSpecificItem(path);
				Session.Delete(directory);

                trx.Commit();
            }

            if (DirectoryDeleted != null)
            {
                DirectoryDeleted.Invoke(this, new FileEventArgs(virtualPath, null));
            }
        }

		private void DeleteDescendants(FileSystemPath path)
		{
			int deletedChunks = Session.CreateQuery("delete from " + typeof(FileSystemChunk).Name + " fsc where fsc.BelongsTo.ID in (select fsi.ID from " + typeof(FileSystemItem).Name + " fsi where fsi.Path.Parent like :parent)")
				.SetParameter("parent", path + "%")
				.ExecuteUpdate();

			int deletedItems = Session.CreateQuery("delete from " + typeof(FileSystemItem).Name + " fsi where fsi.Path.Parent like :parent")
				.SetParameter("parent", path + "%")
				.ExecuteUpdate();
			
			logger.Debug("Deleted " + deletedItems + " items and " + deletedChunks + " chunks below " + path);
		}

        public void CreateDirectory(string virtualPath)
        {
            var path = FileSystemPath.Directory(virtualPath);
            // Ensure consistent behavoir with the System.UI.Directory methods used by mapped and virtual filesystem
            if(DirectoryExists(path.ToString()))
                throw new IOException("The directory " + path.ToString() + " already exists.");

			using (var trx = Session.BeginTransaction())
            {
                CreateDirectoryInternal(virtualPath);
                trx.Commit();
            }

            if (DirectoryCreated != null)
            {
                DirectoryCreated.Invoke(this, new FileEventArgs(virtualPath, null));
            }
        }

        private void CreateDirectoryInternal(string virtualPath)
        {
            var path = FileSystemPath.Directory(virtualPath);

            if (virtualPath != "/")
            {
                EnsureParentExists(path);
            }

            var item = new FileSystemItem
                           {
                               Path = path,
							   Created = Utility.CurrentTime(),
							   Updated = Utility.CurrentTime()
                           };

			Session.Save(item);
        }

        public event EventHandler<FileEventArgs> FileWritten;
        public event EventHandler<FileEventArgs> FileCopied;
        public event EventHandler<FileEventArgs> FileMoved;
        public event EventHandler<FileEventArgs> FileDeleted;
        public event EventHandler<FileEventArgs> DirectoryCreated;
        public event EventHandler<FileEventArgs> DirectoryMoved;
        public event EventHandler<FileEventArgs> DirectoryDeleted;

        private static void CopyStream(Stream input, Stream output)
        {
            var b = new byte[32768];
            int r;
            while ((r = input.Read(b, 0, b.Length)) > 0)
                output.Write(b, 0, r);

            if(!IsStreamSizeOk(output))
                throw new Exception("File with size " + output.Length + " could not be uploaded created because it exceeds the configured UploadFileMaxSize");
        }
    }
}
