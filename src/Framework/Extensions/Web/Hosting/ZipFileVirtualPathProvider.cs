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
using System.Web.Hosting;
using System.Collections.Generic;

namespace Ionic.Zip.Web.VirtualPathProvider
{
	public class ZipFileVirtualPathProvider : System.Web.Hosting.VirtualPathProvider
	{
		ZipFile zipFile;
		Dictionary<string, bool> fileOrDirectoryCache;

		public ZipFileVirtualPathProvider(string zipFilename)
			: base()
		{
			zipFile = ZipFile.Read(zipFilename);
		}

		~ZipFileVirtualPathProvider()
		{
			zipFile.Dispose();
		}

		public override bool FileExists(string virtualPath)
		{
			return Exists(virtualPath, true) ?? Previous.FileExists(virtualPath);
		}

		public override bool DirectoryExists(string virtualDir)
		{
			return Exists(virtualDir, false) ?? Previous.DirectoryExists(virtualDir);
		}

		public override VirtualFile GetFile(string virtualPath)
		{
			ZipEntry zipEntry = GetEntry(virtualPath, true);
			if (zipEntry != null && !zipEntry.IsDirectory)
				return new ZipVirtualFile(virtualPath, zipFile);

			return Previous.GetFile(virtualPath);
		}

		public override VirtualDirectory GetDirectory(string virtualDir)
		{
			ZipEntry zipEntry = GetEntry(virtualDir, false);
			if (zipEntry != null && zipEntry.IsDirectory)
				return new ZipVirtualDirectory(virtualDir, zipFile);

			return Previous.GetDirectory(virtualDir);
		}

		private ZipEntry GetEntry(string virtualPath, bool isFile)
		{
			string zipPath = Util.ConvertVirtualPathToZipPath(virtualPath, isFile);
			return zipFile[zipPath];
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

		private bool? Exists(string virtualDir, bool isFile)
		{
			EnsureCache();

			bool result = false;
			if (fileOrDirectoryCache.TryGetValue(Util.ConvertVirtualPathToZipPath(virtualDir, isFile), out result))
				return result != isFile;

			ZipEntry zipEntry = GetEntry(virtualDir, isFile);
			if (zipEntry != null)
				return isFile != zipEntry.IsDirectory;

			return null;
		}

		void EnsureCache()
		{
			if (fileOrDirectoryCache != null)
				return;

			lock (this)
			{
				if (fileOrDirectoryCache != null)
					return;

				var temp = new Dictionary<string, bool>(StringComparer.InvariantCultureIgnoreCase);
				foreach (var entry in zipFile.Entries)
				{
					temp[entry.FileName] = entry.IsDirectory;
				}
				fileOrDirectoryCache = temp;
			}
		}
	}
}
