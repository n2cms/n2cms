using System.IO;
using System.Linq;
using System.Web.Hosting;

namespace N2.Edit.Tests.FileSystem
{
    public class FakePathProvider : VirtualPathProvider
    {
        readonly string basePath;

        public FakePathProvider(string basePath)
        {
            this.basePath = basePath;
        }

        public string MapPath(string path)
        {
            return Path.Combine(basePath, path.TrimStart('~', '/').Replace('/', '\\'));
        }

        public override bool DirectoryExists(string virtualDir)
        {
            return GetDirectory(virtualDir) != null;
        }
        public override bool FileExists(string virtualPath)
        {
            return GetFile(virtualPath) != null;
        }

        public override VirtualFile GetFile(string virtualPath)
        {
            if (!File.Exists(MapPath(virtualPath))) return null;

            return new VF(virtualPath, this);
        }
        public override VirtualDirectory GetDirectory(string virtualDir)
        {
            if (!Directory.Exists(MapPath(virtualDir))) return null;

            return new VD(virtualDir, this);
        }

        class VF : VirtualFile
        {
            readonly FakePathProvider provider;

            public VF(string path, FakePathProvider provider)
                : base(path.TrimStart('~'))
            {
                this.provider = provider;
            }

            public override Stream Open()
            {
                return System.IO.File.Open(provider.MapPath(VirtualPath), FileMode.OpenOrCreate);
            }
        }
        class VD : VirtualDirectory
        {
            readonly FakePathProvider provider;
            DirectoryInfo dir;
            public VD(string path, FakePathProvider provider) 
                : base(path.TrimStart('~'))
            {
                this.provider = provider;
                dir = new DirectoryInfo(provider.MapPath(path));
            }

            public override System.Collections.IEnumerable Children
            {
                get { return Directories.Cast<VirtualFileBase>().Union(Files.Cast<VirtualFileBase>()).AsEnumerable(); }
            }

            public override System.Collections.IEnumerable Directories
            {
                get { return dir.GetDirectories().Select(d => new VD(VirtualPath + "/" + d.Name, provider)); }
            }

            public override System.Collections.IEnumerable Files
            {
                get { return dir.GetFiles().Select(f => new VF(VirtualPath + "/" + f.Name, provider)); }
            }
        }
    }
}
