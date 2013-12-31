using System;
using System.IO;
using N2.Edit.FileSystem;

namespace N2.Edit.Tests.FileSystem
{
    public class FakeVppFileSystem : VirtualPathFileSystem
    {
        public string BasePath = AppDomain.CurrentDomain.BaseDirectory;

        protected override DirectoryInfo GetDirectoryInfo(string virtualDir)
        {
            return new DirectoryInfo(MapPath(virtualDir));
        }

        protected override FileInfo GetFileInfo(string virtualPath)
        {
            return new FileInfo(MapPath(virtualPath));
        }

        protected override string MapPath(string virtualDir)
        {
            return Path.Combine(BasePath, virtualDir.TrimStart('~', '/').Replace('/', '\\'));
        }
    }
}
