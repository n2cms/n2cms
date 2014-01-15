using System;
using System.IO;
using N2.Edit.FileSystem;

namespace N2.Tests.Fakes
{
    public class FakeMappedFileSystem : MappedFileSystem
    {
        public string BasePath = AppDomain.CurrentDomain.BaseDirectory;

        protected override string MapPath(string virtualPath)
        {
            return Path.Combine(BasePath, virtualPath.TrimStart('~', '/').Replace('/', '\\'));
        }
    }
}
