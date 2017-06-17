using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using N2.Engine;
using System.Threading;
using System.Linq;

namespace N2.Edit.FileSystem
{
    /// <summary>
    /// Uses HttpContext.Current and System.IO to operate files.
    /// </summary>
    [Service(typeof(IFileSystem))]
    public class MappedFileSystem : IFileSystem
    {
        #region IFileSystem Members

        public IEnumerable<FileData> GetFiles(string parentVirtualPath)
        {
            string path = MapPath(parentVirtualPath);
            if (!Directory.Exists(path))
                yield break;
            foreach (var file in new DirectoryInfo(path).GetFiles())
                if(!file.Name.StartsWith("."))
                    yield return GetFile(N2.Web.Url.Combine(parentVirtualPath, file.Name), file);
        }

        public FileData GetFile(string virtualPath)
        {
            var info = new FileInfo(MapPath(virtualPath));
            return GetFile(virtualPath, info);
        }

        private static FileData GetFile(string virtualPath, FileInfo info)
        {

            if (!info.Exists)
                return null;

            return new FileData
            {
                Name = info.Name,
                Created = GetSafely(info, i => i.CreationTime),
                Updated = GetSafely(info, i => i.LastWriteTime),
                VirtualPath = virtualPath,
                Length = GetSafely(info, i => i.Length)
            };
        }

        public IEnumerable<DirectoryData> GetDirectories(string parentVirtualPath)
        {
            string path = MapPath(parentVirtualPath);
            if (!Directory.Exists(path))
                yield break;
            foreach (var dir in new DirectoryInfo(path).GetDirectories())
                if (!dir.Name.StartsWith("."))
                    yield return GetDirectory(N2.Web.Url.Combine(parentVirtualPath, dir.Name), dir);
        }

        public DirectoryData GetDirectory(string virtualPath)
        {
            DirectoryInfo info = new DirectoryInfo(MapPath(virtualPath));
            return GetDirectory(virtualPath, info);
        }

        private static DirectoryData GetDirectory(string virtualPath, DirectoryInfo info)
        {
            if (!info.Exists)
                return null;

            return new DirectoryData 
            { 
                Name = info.Name,
                Created = GetSafely(info, i => i.CreationTime),
                Updated = GetSafely(info, i => i.LastWriteTime),
                VirtualPath = virtualPath
            };
        }

        /// <summary>Searches for files in all Upload Directories.</summary>
        /// <param name="query">The search term</param>
        /// <param name="uploadDirectories">All Upload Directories</param>
        /// <returns>An enumeration of files matching the query.</returns>
        public virtual IEnumerable<FileData> SearchFiles(string query, List<Collections.HierarchyNode<ContentItem>> uploadDirectories)
        {
            if (query.IndexOf('*') < 0) { query = "*" + query + "*"; }

            var resultFilenames = new List<string>();
            var resultFileData = new List<FileData>();

            foreach (var dir in uploadDirectories)
            {
                var d = System.Web.Hosting.HostingEnvironment.MapPath(HttpContext.Current.Request.ApplicationPath + dir.Current.Path);

                //Returns Absolute Paths
                var results = Directory.GetFiles(d, query, SearchOption.AllDirectories);

                //Rebase to VirtualPaths
                resultFilenames.AddRange(results.ToList().Select(p => p.Replace(d, dir.Current.Url).Replace('\\','/')));
            }

            foreach(var virtualPath in resultFilenames)
            {
                if (string.IsNullOrEmpty(virtualPath)) continue;

                var f = GetFile(virtualPath);
                if (f == null) continue;

                resultFileData.Add(f);
            }

            return resultFileData;
            
        }


        public bool FileExists(string virtualPath)
        {
            return File.Exists(MapPath(virtualPath));
        }

        public void MoveFile(string fromVirtualPath, string destinationVirtualPath)
        {
            MoveFileInternal(fromVirtualPath, destinationVirtualPath);

            if (FileMoved != null)
                FileMoved.Invoke(this, new FileEventArgs(destinationVirtualPath, fromVirtualPath));
        }

        private void MoveFileInternal(string fromVirtualPath, string destinationVirtualPath)
        {
            File.Move(MapPath(fromVirtualPath), MapPath(destinationVirtualPath));
        }

        public void DeleteFile(string virtualPath)
        {
            DeleteFileInternal(virtualPath);

            if (FileDeleted != null)
                FileDeleted.Invoke(this, new FileEventArgs(virtualPath, null));
        }

        private void DeleteFileInternal(string virtualPath)
        {
            File.Delete(MapPath(virtualPath));
        }

        public void CopyFile(string fromVirtualPath, string destinationVirtualPath)
        {
            CopyFileInternal(fromVirtualPath, destinationVirtualPath);

            if (FileCopied != null)
                FileCopied.Invoke(this, new FileEventArgs(destinationVirtualPath, fromVirtualPath));
        }

        private void CopyFileInternal(string fromVirtualPath, string destinationVirtualPath)
        {
            File.Copy(MapPath(fromVirtualPath), MapPath(destinationVirtualPath));
        }

        public System.IO.Stream OpenFile(string virtualPath, bool readOnly = false)
        {
            FileAccess access = readOnly ? FileAccess.Read : FileAccess.ReadWrite;
            FileShare share = readOnly ? FileShare.Read : FileShare.None;

            return File.Open(MapPath(virtualPath), FileMode.OpenOrCreate, access, share);
        }

        public void WriteFile(string virtualPath, System.IO.Stream inputStream)
        {
            string path = MapPath(virtualPath);
            if (!Directory.Exists(Path.GetDirectoryName(path)))
                Directory.CreateDirectory((Path.GetDirectoryName(path)));
            
            if (FileExists(virtualPath))
            {
                ReplaceFile(virtualPath, inputStream);
            }
            else
            {
                CreateFile(virtualPath, inputStream);
            }

            if (FileWritten != null)
                FileWritten.Invoke(this, new FileEventArgs(virtualPath, null));
        }

        private void ReplaceFile(string virtualPath, System.IO.Stream inputStream)
        {
            string tempFile = virtualPath + "." + Path.GetRandomFileName();
            try
            {
                CreateFile(tempFile, inputStream);
                DeleteFileInternal(virtualPath);
                MoveFileInternal(tempFile, virtualPath);
            }
            finally
            {
                DeleteFile(tempFile);
            }
        }

        private void CreateFile(string virtualPath, System.IO.Stream inputStream)
        {
            var path = MapPath(virtualPath);
            var dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            using (var s = File.Create(path))

            {
                TransferBetweenStreams(inputStream, s);
            }
        }

        public void ReadFileContents(string virtualPath, System.IO.Stream outputStream)
        {
            using(var s = File.OpenRead(MapPath(virtualPath)))
            {
                TransferBetweenStreams(s, outputStream);
            }
        }

        public bool DirectoryExists(string virtualPath)
        {
            return Directory.Exists(MapPath(virtualPath));
        }

        public void MoveDirectory(string fromVirtualPath, string destinationVirtualPath)
        {
            try
            {
                Directory.Move(MapPath(fromVirtualPath), MapPath(destinationVirtualPath));
            }
            catch (IOException)
            {
                // retry once
                Thread.Sleep(10);
                Directory.Move(MapPath(fromVirtualPath), MapPath(destinationVirtualPath));
            }

            if (DirectoryMoved != null)
                DirectoryMoved.Invoke(this, new FileEventArgs(destinationVirtualPath, fromVirtualPath));
        }

        public void DeleteDirectory(string virtualPath)
        {
            try
            {
                Directory.Delete(MapPath(virtualPath), true);
            }
            catch (IOException)
            {
                // retry once
                Thread.Sleep(10);
                Directory.Delete(MapPath(virtualPath), true);
            }

            if (DirectoryDeleted != null)
                DirectoryDeleted.Invoke(this, new FileEventArgs(virtualPath, null));
        }

        public void CreateDirectory(string virtualPath)
        {
            Directory.CreateDirectory(MapPath(virtualPath));

            if (DirectoryCreated != null)
                DirectoryCreated.Invoke(this, new FileEventArgs(virtualPath, null));
        }

        public event EventHandler<FileEventArgs> FileWritten;

        public event EventHandler<FileEventArgs> FileCopied;

        public event EventHandler<FileEventArgs> FileMoved;

        public event EventHandler<FileEventArgs> FileDeleted;

        public event EventHandler<FileEventArgs> DirectoryCreated;

        public event EventHandler<FileEventArgs> DirectoryMoved;

        public event EventHandler<FileEventArgs> DirectoryDeleted;

        #endregion

        protected virtual void TransferBetweenStreams(Stream inputStream, Stream outputStream)
        {
            byte[] buffer = new byte[32768];
            while (true)
            {
                int bytesRead = inputStream.Read(buffer, 0, buffer.Length);
                if (bytesRead <= 0)
                    break;

                outputStream.Write(buffer, 0, bytesRead);
            }
        }

        protected virtual string MapPath(string virtualPath)
        {
            return System.Web.Hosting.HostingEnvironment.MapPath(virtualPath);
        }

        protected virtual string AbsolutePathToVirtual(string absolutePath)
        {
            var hs = System.Web.Hosting.HostingEnvironment.MapPath(HttpContext.Current.Request.ApplicationPath);
            if (absolutePath.IndexOf(hs) != 0) return null;

            return ("/" + absolutePath.Substring(hs.Length).Replace('\\', '/')).Replace("//", "/");
        }

        private static T GetSafely<K, T>(K value, Func<K, T> getter)
        {
            try
            {
                return getter(value);
            }
            catch (Exception)
            {
                return default(T);
            }
        }
    }
}
