/**
Thu, 12 Feb 2009  13:34

This is an ASP.NET VirtualPathProvider that uses DotNetZip to serve an
ASP.NET website out of a ZIP file. 

With this VPP, you can zip-up your website content - ASPX, images,
config files, everything - and serve the content directly out of that
ZIP file.  This means deploying a new version of the website is simply a
matter of copying a single new ZIP file. 

Very slick.

If you have ever seen Java's WAR file deployment mechanism, this is very similar. 

This VPP comes out of an example originally constructed by Clarius
Consulting, that uses SharpZipLib. 
http://msdn.microsoft.com/en-us/library/Aa479502.aspx

This VPP works the same way, in fact there is very little code change
from the original - just some simplification.

You're free to use this VPP under the same license as DotNetZip.  
**/

using System;
//using System.Collections.Generic;
//using System.Text;
using System.Web.Hosting;

using Ionic.Zip;

namespace Ionic.Zip.Web.VirtualPathProvider
{
    public class ZipFileVirtualPathProvider : System.Web.Hosting.VirtualPathProvider
    {
        ZipFile _zipFile;

        public ZipFileVirtualPathProvider (string zipFilename)
            : base () {
            _zipFile =  ZipFile.Read(zipFilename);
        }

        ~ZipFileVirtualPathProvider () {
            _zipFile.Dispose ();
        }

        public override bool FileExists (string virtualPath)
		{
			ZipEntry zipEntry = GetEntry(virtualPath, true);

			if (zipEntry != null)
			{
				//if (zipEntry.IsDirectory)
				//{
				//    zipPath += "Default.aspx";
				//    zipEntry = _zipFile[zipPath];
				//    if (zipEntry == null)
				//        return false;
				//}
				return !zipEntry.IsDirectory;
			}

			return Previous.FileExists(virtualPath);
		}

		public override bool DirectoryExists (string virtualDir)
		{
			ZipEntry zipEntry = GetEntry(virtualDir, false);

			if (zipEntry != null)
			{
				return zipEntry.IsDirectory;
			}

			return Previous.DirectoryExists(virtualDir);
		}

        public override VirtualFile GetFile (string virtualPath)
		{
			ZipEntry zipEntry = GetEntry(virtualPath, true);
			if (zipEntry != null && !zipEntry.IsDirectory)
				return new ZipVirtualFile(virtualPath, _zipFile);

			return Previous.GetFile(virtualPath);

			////string zipPath = Util.ConvertVirtualPathToZipPath(virtualPath, false);
			////ZipEntry zipEntry = _zipFile[zipPath];
			////if (zipEntry != null && zipEntry.IsDirectory)
			////    virtualPath += "Default.aspx";
			//return new ZipVirtualFile (virtualPath, _zipFile);
        }

		public override VirtualDirectory GetDirectory (string virtualDir)
		{
			ZipEntry zipEntry = GetEntry(virtualDir, false);
			if (zipEntry != null && zipEntry.IsDirectory)
				return new ZipVirtualDirectory(virtualDir, _zipFile);

			return Previous.GetDirectory(virtualDir);
		}

		private ZipEntry GetEntry(string virtualPath, bool isFile)
		{
			string zipPath = Util.ConvertVirtualPathToZipPath(virtualPath, isFile);
			return _zipFile[zipPath];
		}

        public override string GetFileHash(string virtualPath, System.Collections.IEnumerable virtualPathDependencies)
        {
			if (Previous.FileExists(virtualPath))
				return Previous.GetFileHash(virtualPath, virtualPathDependencies);
			return null;
		}

        public override System.Web.Caching.CacheDependency GetCacheDependency(String virtualPath, System.Collections.IEnumerable virtualPathDependencies, DateTime utcStart)
		{
			if (Previous.FileExists(virtualPath))
				return Previous.GetCacheDependency(virtualPath, virtualPathDependencies, utcStart);
			return null;
        }
    }
}
