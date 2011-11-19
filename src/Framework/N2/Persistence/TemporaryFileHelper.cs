using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Engine;
using System.IO;
using N2.Web;

namespace N2.Persistence
{
    [Service]
    public class TemporaryFileHelper
    {
        private IHost host;

        public TemporaryFileHelper(IHost host)
        {
            this.host = host;
        }

        public virtual string GetTemporaryDirectory()
        {
            return Path.GetTempPath();
        }

        public virtual string GetCurrentSiteTemporaryDirectory()
        {
            string dir = Path.Combine(GetTemporaryDirectory(), "n2cms\\site_" + host.CurrentSite.Authority.Replace('.', '_'));
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            return dir;
        }

        public virtual string GetSharedTemporaryDirectory()
        {
            string dir = Path.Combine(GetTemporaryDirectory(), "n2cms");
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            return dir;
        }

        public virtual string GetTemporaryFileName(string extension)
        {
            string dir = GetSharedTemporaryDirectory();
            return Path.Combine(dir, Path.GetTempFileName() + extension);
        }

        public virtual void ClearTemporaryFile(TimeSpan olderThan)
        {
            DeleteEmptyDirectoriesRecursive(GetSharedTemporaryDirectory(), olderThan);
        }

        private bool DeleteEmptyDirectoriesRecursive(string rootDir, TimeSpan olderThan)
        {
            bool allDeleted = true;
            var di = new DirectoryInfo(rootDir);
            foreach(var d in di.GetDirectories())
                allDeleted &= DeleteEmptyDirectoriesRecursive(d.FullName, olderThan);

            foreach (var fi in di.GetFileSystemInfos())
            {
                if(fi.LastWriteTime < DateTime.Now.Subtract(olderThan))
                    fi.Delete();
                else
                    allDeleted = false;
            }

            if (allDeleted)
                Directory.Delete(rootDir);
            
            return allDeleted;
        }
    }
}
