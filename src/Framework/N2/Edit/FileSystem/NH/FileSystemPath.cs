using System;

namespace N2.Edit.FileSystem.NH
{
    /// <summary>
    /// A path component used by <see cref="FileSystemItem"/>.
    /// </summary>
    public class FileSystemPath
    {
        public string Parent { get; set; }
        public string Name { get; set; }
        public bool IsDirectory { get; set; }

        public FileSystemPath()
        {
        }

        private FileSystemPath(string virtualPath)
        {
            var sanitizedPath = virtualPath.TrimStart('~').TrimEnd('/').Replace('\\', '/');
            var lastSlash = sanitizedPath.LastIndexOf('/') + 1;

            Parent = sanitizedPath.Substring(0, lastSlash);
            Name = sanitizedPath.Substring(lastSlash);
        }

        public static FileSystemPath File(string virtualPath)
        {
            return new FileSystemPath(virtualPath) { IsDirectory = false };
        }

        public static FileSystemPath Directory(string virtualPath)
        {
            return new FileSystemPath(virtualPath) { IsDirectory = true };
        }

        public override string ToString()
        {
            var path = Parent + Name;
            if (IsDirectory) path += '/';
            return path;
        }

        public bool IsDescendantOf(FileSystemPath source)
        {
            return Parent.StartsWith(source.ToString());
        }

        public void Rebase(FileSystemPath source, FileSystemPath target)
        {
            if (!source.IsDirectory || !target.IsDirectory)
            {
                throw new ApplicationException("Rebase parameters \"source\" and \"target\" should both be directory paths.");
            }

            Parent = new FileSystemPath(target + ToString().Substring(source.ToString().Length)).Parent;
        }
    }

}
