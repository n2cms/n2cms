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

namespace N2.Edit.FileSystem.NH
{
    public class DatabaseFileSystem : IFileSystem, IAutoStart
    {
        private readonly ISessionProvider _sessionProvider;

		#region class DatabaseFileSystemStream
		private class DatabaseFileSystemStream : MemoryStream
        {
            private readonly FileSystemItem attachedTo;
            private readonly DatabaseFileSystem fileSys;
            private bool changed;

            public DatabaseFileSystemStream(FileSystemItem attachedTo, DatabaseFileSystem fileSys, byte [] data)
            {
                if(attachedTo==null) throw new Exception("attachedTo must not be null");
                if(fileSys == null) throw new Exception("fileSys most not be null");

                this.attachedTo = attachedTo;
                this.fileSys = fileSys;
                
                base.Write(data, 0, data.Length);
                Position = 0;
            }

            public override sealed long Position
            {
                get { return base.Position; }
                set { base.Position = value; }
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
                    fileSys.UpdateFile(attachedTo.Path, this);

                base.Flush();
            }

            protected override void  Dispose(bool disposing)
            {
                Flush();
                base.Dispose(disposing);
            }
        }
		#endregion

		public DatabaseFileSystem(ISessionProvider sessionProvider)
        {
            this._sessionProvider = sessionProvider;
        }

        private IEnumerable<FileSystemItem> FindChildren(Path path, ICriterion criterion)
        {
            return _sessionProvider.OpenSession.Session
                .CreateCriteria<FileSystemItem>()
                .Add(Restrictions.Eq("Path.Parent", path.ToString()))
                .Add(criterion)
                .List<FileSystemItem>();
        }

        private FileSystemItem GetSpecificItem(Path path)
        {
            return _sessionProvider.OpenSession.Session
                .CreateCriteria<FileSystemItem>()
                .Add(Restrictions.Eq("Path", path))
                .UniqueResult<FileSystemItem>();
        }

        private void EnsureParentExists(Path target)
        {
            if(!DirectoryExists(target.Parent))
            {
                _CreateDirectory(target.Parent);
            }
        }

        private void AssertParentExists(Path target)
        {
            var uploadFolders = new EditSection().UploadFolders;
            if (uploadFolders.Folders.Any(uploadFolder => Path.Directory(uploadFolder).ToString() == target.Parent))
                EnsureParentExists(target);

            if (!DirectoryExists(target.Parent))
            {
                throw new DirectoryNotFoundException("Destination directory not found: " + target.Parent);
            }
        }

        public IEnumerable<FileData> GetFiles(string parentVirtualPath)
        {
            var path = Path.Directory(parentVirtualPath);

            return from child in FindChildren(path, Restrictions.IsNotNull("Length"))
                   select child.ToFileData();
        }

        public FileData GetFile(string virtualPath)
        {
            var path = Path.File(virtualPath);
            return GetSpecificItem(path).ToFileData();
        }

        public IEnumerable<DirectoryData> GetDirectories(string parentVirtualPath)
        {
            var path = Path.Directory(parentVirtualPath);

            return from child in FindChildren(path, Restrictions.IsNull("Length"))
                   select child.ToDirectoryData();
        }

        public DirectoryData GetDirectory(string virtualPath)
        {
            var path = Path.Directory(virtualPath);
            var item = GetSpecificItem(path);

            return item == null
                ? new DirectoryData { VirtualPath = path.ToString() }
                : item.ToDirectoryData();
        }

        public bool FileExists(string virtualPath)
        {
            var path = Path.File(virtualPath);
            var item = GetSpecificItem(path);
            return item != null && item.Length.HasValue;
        }

        public void MoveFile(string fromVirtualPath, string destinationVirtualPath)
        {
            var source = Path.File(fromVirtualPath);
            var target = Path.File(destinationVirtualPath);

            using (var trx = _sessionProvider.OpenSession.Session.BeginTransaction())
            {
                AssertParentExists(target);

                var file = GetSpecificItem(source);
                file.Path = target;
                trx.Commit();
            }

            if (FileMoved != null)
            {
                FileMoved.Invoke(this, new FileEventArgs(destinationVirtualPath, fromVirtualPath));
            }
        }

        public void DeleteFile(string virtualPath)
        {
            using (var trx = _sessionProvider.OpenSession.Session.BeginTransaction())
            {
                var path = Path.File(virtualPath);
                var item = GetSpecificItem(path);
                _sessionProvider.OpenSession.Session.Delete(item);
                trx.Commit();
            }

            if (FileDeleted != null)
            {
                FileDeleted.Invoke(this, new FileEventArgs(virtualPath, null));
            }
        }

        public void CopyFile(string fromVirtualPath, string destinationVirtualPath)
        {
            var source = Path.File(fromVirtualPath);
            var target = Path.File(destinationVirtualPath);

            using (var trx = _sessionProvider.OpenSession.Session.BeginTransaction())
            {
                AssertParentExists(target);

                var file = GetSpecificItem(source);

                var copy = new FileSystemItem
                {
                    Path = target,
                    Data = file.Data,
                    Length = file.Length,
                    Created = DateTime.Now
                };

                _sessionProvider.OpenSession.Session.Save(copy);
                trx.Commit();
            }

            if (FileCopied != null)
            {
                FileCopied.Invoke(this, new FileEventArgs(destinationVirtualPath, fromVirtualPath));
            }
        }

        public Stream OpenFile(string virtualPath)
        {
            var path = Path.File(virtualPath);

            if(!FileExists(path.ToString()))
            {
                CreateFile(path, null);
            }
            var blob = GetSpecificItem(path);

            var stream = new DatabaseFileSystemStream(blob, this, blob.Data);
            return stream;
        }

        public void WriteFile(string virtualPath, Stream inputStream)
        {
            if (!FileExists(virtualPath))
            {
                CreateFile(Path.File(virtualPath), inputStream);
            }
            else
            {
                UpdateFile(Path.File(virtualPath), inputStream);
            }

            if (FileWritten != null)
            {
                FileWritten.Invoke(this, new FileEventArgs(virtualPath, null));
            }
        }

        private void UpdateFile(Path virtualPath, Stream inputStream)
        {
            var file = GetSpecificItem(virtualPath);
                
            using (var trx = _sessionProvider.OpenSession.Session.BeginTransaction())
            using (var buffer = new MemoryStream())
            {
                if (inputStream != null)
                {
                    inputStream.Position = 0;
                    CopyStream(inputStream, buffer);
                    inputStream.Position = 0;
                }

                file.Data = buffer.GetBuffer();
                file.Created = DateTime.Now;
                file.Length = buffer.Length;
                _sessionProvider.OpenSession.Session.Update(file);
                trx.Commit();
            }
        }

        private void CreateFile(Path virtualPath, Stream inputStream)
        {
            using (var trx = _sessionProvider.OpenSession.Session.BeginTransaction())
            using (var buffer = new MemoryStream())
            {
                AssertParentExists(virtualPath);

                if (inputStream != null)
                {
                    CopyStream(inputStream, buffer);
                    inputStream.Position = 0;
                }

                var item = new FileSystemItem
                               {
                                   Path = virtualPath,
                                   Data = buffer.GetBuffer(),
                                   Created = DateTime.Now,
                                   Length = buffer.Length
                               };

                _sessionProvider.OpenSession.Session.Save(item);
                trx.Commit();
            }
        }

        public void ReadFileContents(string virtualPath, Stream outputStream)
        {
            var path = Path.File(virtualPath);
            var blob = GetSpecificItem(path);
            outputStream.Write(blob.Data, 0, blob.Data.Length);
        }

        public bool DirectoryExists(string virtualPath)
        {
            var path = Path.Directory(virtualPath);
            var item = GetSpecificItem(path);
            return item != null && !item.Length.HasValue;
        }

        public void MoveDirectory(string fromVirtualPath, string destinationVirtualPath)
        {
            var source = Path.Directory(fromVirtualPath);
            var target = Path.Directory(destinationVirtualPath);

            if (target.IsDescendantOf(source))
            {
                throw new ApplicationException("Cannot move directory into own subdictory.");
            }

            using (var trx = _sessionProvider.OpenSession.Session.BeginTransaction())
            {
                var directory = GetSpecificItem(source);
                var descendants = _sessionProvider.OpenSession.Session
                    .CreateCriteria<FileSystemItem>()
                    .Add(Restrictions.Like("Path.Parent", directory.Path.ToString(), MatchMode.Start))
                    .List<FileSystemItem>();

                foreach (var item in descendants)
                {
                    item.Path.Rebase(source, target);
                }

                directory.Path = target;

                trx.Commit();
            }

            if (DirectoryMoved != null)
            {
                DirectoryMoved.Invoke(this, new FileEventArgs(destinationVirtualPath, fromVirtualPath));
            }
        }

        public void DeleteDirectory(string virtualPath)
        {
            var path = Path.Directory(virtualPath);

            using (var trx = _sessionProvider.OpenSession.Session.BeginTransaction())
            {
                var directory = GetSpecificItem(path);

                _sessionProvider.OpenSession.Session
                    .CreateQuery("delete from N2.Edit.FileSystem.NH.FileSystemItem where Path.Parent like :parent")
                    .SetParameter("parent", path + "%")
                    .ExecuteUpdate();

                _sessionProvider.OpenSession.Session.Delete(directory);

                trx.Commit();
            }

            if (DirectoryDeleted != null)
            {
                DirectoryDeleted.Invoke(this, new FileEventArgs(virtualPath, null));
            }
        }

        public void CreateDirectory(string virtualPath)
        {
            var path = Path.Directory(virtualPath);
            // Ensure consistent behavoir with the System.UI.Directory methods used by mapped and virtual filesystem
            if(DirectoryExists(path.ToString()))
                throw new IOException("The directory " + path.ToString() + " already exists.");

            using (var trx = _sessionProvider.OpenSession.Session.BeginTransaction())
            {
                _CreateDirectory(virtualPath);
                trx.Commit();
            }

            if (DirectoryCreated != null)
            {
                DirectoryCreated.Invoke(this, new FileEventArgs(virtualPath, null));
            }
        }

        private void _CreateDirectory(string virtualPath)
        {
            var path = Path.Directory(virtualPath);

            if (virtualPath != "/")
            {
                EnsureParentExists(path);
            }

            var item = new FileSystemItem
                           {
                               Path = path,
                               Created = DateTime.Now
                           };

            _sessionProvider.OpenSession.Session.Save(item);
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
        }

        public void Start()
        {
            EventBroker.Instance.PreRequestHandlerExecute += UploadFileHttpHandler.HttpApplication_PreRequestHandlerExecute;
        }

        public void Stop()
        {
            EventBroker.Instance.PreRequestHandlerExecute -= UploadFileHttpHandler.HttpApplication_PreRequestHandlerExecute;
        }
    }
}
