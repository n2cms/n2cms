using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Edit.FileSystem.NH
{
    public class Path
    {
        public string Parent { get; private set; }
        public string Name { get; private set; }
        private bool _isDirectory;

        private Path(string virtualPath)
        {
            var sanitizedPath = virtualPath.TrimStart('~').TrimEnd('/').Replace('\\', '/');
            var lastSlash = sanitizedPath.LastIndexOf('/') + 1;

            Parent = sanitizedPath.Substring(0, lastSlash);
            Name = sanitizedPath.Substring(lastSlash);
        }

        public static Path File(string virtualPath)
        {
            return new Path(virtualPath) { _isDirectory = false };
        }

        public static Path Directory(string virtualPath)
        {
            return new Path(virtualPath) { _isDirectory = true };
        }

        public override string ToString()
        {
            var path = Parent + Name;
            if (_isDirectory) path += '/';
            return path;
        }

        public bool IsDescendantOf(Path source)
        {
            return Parent.StartsWith(source.ToString());
        }
    }

}
