using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Engine;
using System.IO;
using N2.Web;

namespace N2.Persistence
{
	/// <summary>
	/// Helps creating temporary files which are shared between sites.
	/// </summary>
	public class BasicTemporaryFileHelper
	{
		private IWebContext webContext;

		public BasicTemporaryFileHelper(IWebContext context)
		{
			this.webContext = context;

		}

		protected virtual string GetTemporaryDirectory()
		{
			return Path.Combine(webContext.MapPath("~/App_Data/"), "Temp");
		}

		public virtual string GetSharedTemporaryDirectory()
		{
			string dir = Path.Combine(GetTemporaryDirectory(), "shared");
			if (!Directory.Exists(dir))
				Directory.CreateDirectory(dir);

			return dir;
		}

		public virtual string GetTemporaryFileName(string extension)
		{
			string dir = GetSharedTemporaryDirectory();
			return Path.Combine(dir, Path.GetFileNameWithoutExtension(Path.GetTempFileName()) + extension);
		}

		public virtual void ClearTemporaryFiles(TimeSpan olderThan)
		{
			DeleteEmptyDirectoriesRecursive(GetSharedTemporaryDirectory(), olderThan);
		}

		private bool DeleteEmptyDirectoriesRecursive(string rootDir, TimeSpan olderThan)
		{
			bool allDeleted = true;
			var di = new DirectoryInfo(rootDir);
			foreach (var d in di.GetDirectories())
				allDeleted &= DeleteEmptyDirectoriesRecursive(d.FullName, olderThan);

			foreach (var fi in di.GetFileSystemInfos())
			{
				if (fi.LastWriteTime < DateTime.Now.Subtract(olderThan))
					fi.Delete();
				else
					allDeleted = false;
			}

			if (allDeleted)
				Directory.Delete(rootDir);

			return allDeleted;
		}
	}

	/// <summary>
	/// Extends <see cref="BasicTemporaryFileHelper"/> with functionality for accessing temporary folder for the curren site.
	/// </summary>
    [Service]
    public class TemporaryFileHelper : BasicTemporaryFileHelper
    {
        private IHost host;

        public TemporaryFileHelper(IHost host, IWebContext webContext)
			: base(webContext)
        {
            this.host = host;
        }

        public virtual string GetCurrentSiteTemporaryDirectory()
        {
            string dir = Path.Combine(GetTemporaryDirectory(), "site_" + host.CurrentSite.Authority.Replace('.', '_'));
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            return dir;
        }
    }
}
