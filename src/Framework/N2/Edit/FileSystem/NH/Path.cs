using System;

namespace N2.Edit.FileSystem.NH
{
    public class Path
    {
        public string Parent { get; set; }
        public string Name { get; set; }
		public bool IsDirectory { get; set; }

        public Path(){}

        private Path(string virtualPath)
        {
            var sanitizedPath = virtualPath.TrimStart('~').TrimEnd('/').Replace('\\', '/');
            var lastSlash = sanitizedPath.LastIndexOf('/') + 1;

            Parent = sanitizedPath.Substring(0, lastSlash);
            Name = sanitizedPath.Substring(lastSlash);
        }

        public static Path File(string virtualPath)
        {
            return new Path(virtualPath) { IsDirectory = false };
        }

        public static Path Directory(string virtualPath)
        {
            return new Path(virtualPath) { IsDirectory = true };
        }

        public override string ToString()
        {
            var path = Parent + Name;
            if (IsDirectory) path += '/';
            return path;
        }

        public bool IsDescendantOf(Path source)
        {
            return Parent.StartsWith(source.ToString());
        }

        public void Rebase(Path source, Path target)
        {
            if (!source.IsDirectory || !target.IsDirectory)
            {
                throw new ApplicationException("Rebase parameters \"source\" and \"target\" should both be directory paths.");
            }

            Parent = new Path(target + ToString().Substring(source.ToString().Length)).Parent;
        }
	}

}
