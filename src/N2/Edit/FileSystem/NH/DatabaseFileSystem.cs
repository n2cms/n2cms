using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using N2.Persistence.NH;
using NHibernate.Criterion;

namespace N2.Edit.FileSystem.NH
{
    public class DatabaseFileSystem : IFileSystem
    {
        private readonly ISessionProvider _sessionProvider;

        public DatabaseFileSystem(ISessionProvider sessionProvider)
        {
            _sessionProvider = sessionProvider;
        }

        private IEnumerable<FileSystemItem> FindChildren(Path path, ICriterion criterion)
        {
            return _sessionProvider.OpenSession.Session
                .CreateCriteria<FileSystemItem>()
                .Add(Restrictions.Eq("Path.Parent", path))
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

        private void AssertParentExists(Path target)
        {
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
            var blob = GetSpecificItem(path);
            return new MemoryStream(blob.Data);
        }

        public void WriteFile(string virtualPath, Stream inputStream)
        {
            var path = Path.File(virtualPath);

            using (var trx = _sessionProvider.OpenSession.Session.BeginTransaction())
            using (var buffer = new MemoryStream())
            {
                AssertParentExists(path);

                CopyStream(inputStream, buffer);

                var item = new FileSystemItem
                {
                    Path = path,
                    Data = buffer.GetBuffer(),
                    Created = DateTime.Now,
                    Length = buffer.Length
                };

                _sessionProvider.OpenSession.Session.Save(item);
                trx.Commit();
            }

            if (FileWritten != null)
            {
                FileWritten.Invoke(this, new FileEventArgs(virtualPath, null));
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
                    .Add(Restrictions.Like("Path.Parent", directory.Path.Parent, MatchMode.Start))
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

            using (var trx = _sessionProvider.OpenSession.Session.BeginTransaction())
            {
                AssertParentExists(path);

                var item = new FileSystemItem
                {
                    Path = path,
                    Created = DateTime.Now
                };

                _sessionProvider.OpenSession.Session.Save(item);
                trx.Commit();
            }

            if (DirectoryCreated != null)
            {
                DirectoryCreated.Invoke(this, new FileEventArgs(virtualPath, null));
            }
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
    }
}
