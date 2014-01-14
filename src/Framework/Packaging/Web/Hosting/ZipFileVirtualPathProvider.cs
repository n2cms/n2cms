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
using System.Linq;
using System.Web.Hosting;
using System.Collections.Generic;
using System.Web;
using System.IO;
using System.Text.RegularExpressions;
using System.Web.Caching;
using System.Collections;
using System.Diagnostics;
using N2.Web;
using ICSharpCode.SharpZipLib.Zip;

namespace SharpZipLib.Web.VirtualPathProvider
{
    public class ZipFileVirtualPathProvider : System.Web.Hosting.VirtualPathProvider
    {
        ZipFile zipFile;
        string zipFilePath;
        Dictionary<string, bool> fileOrDirectoryCache;

        public ZipFileVirtualPathProvider(string zipFilename)
            : base()
        {
            this.zipFilePath = zipFilename;
            zipFile = new ZipFile(zipFilename);
        }

        ~ZipFileVirtualPathProvider()
        {
            ((IDisposable)zipFile).Dispose();
        }

        public override bool FileExists(string virtualPath)
        {
            //virtualPath = ToRelative(virtualPath);
            bool exists = Exists(virtualPath, true) || Previous.FileExists(virtualPath);
            N2.Engine.Logger.Debug("ZipVPP: " + exists + " " + virtualPath);
            return exists;
        }

        public override bool DirectoryExists(string virtualDir)
        {
            //virtualDir = ToRelative(virtualDir);
            bool exists = Exists(virtualDir, false) || Previous.DirectoryExists(virtualDir);
            return exists;
        }

        public override VirtualFile GetFile(string virtualPath)
        {
            //virtualPath = ToRelative(virtualPath);
            if (Exists(virtualPath, true))
            {
                ZipEntry zipEntry = GetEntry(virtualPath, true);
                if (zipEntry != null && !zipEntry.IsDirectory)
                    return new ZipVirtualFile(virtualPath, zipFile);
            }
            return Previous.GetFile(virtualPath);
        }

        public override VirtualDirectory GetDirectory(string virtualDir)
        {
            //virtualDir = ToRelative(virtualDir);
            if (Exists(virtualDir, false))
            {
                ZipEntry zipEntry = GetEntry(virtualDir, false);
                if (zipEntry != null && zipEntry.IsDirectory)
                    return new ZipVirtualDirectory(virtualDir, zipFile);
            }
            return Previous.GetDirectory(virtualDir);
        }

        public override string GetFileHash(string virtualPath, System.Collections.IEnumerable virtualPathDependencies)
        {
            //virtualPath = ToRelative(virtualPath);
            if (Exists(virtualPath, true))
                return zipFilePath + File.GetLastWriteTimeUtc(zipFilePath) + virtualPath;
            
            return Previous.GetFileHash(virtualPath, virtualPathDependencies);
        }

        public override CacheDependency GetCacheDependency(String virtualPath, IEnumerable virtualPathDependencies, DateTime utcStart)
        {
            //virtualPath = ToRelative(virtualPath);
            var filesNotBelongingToZip = virtualPathDependencies.OfType<string>().Select(f => ToRelative(f)).Where(f => !Exists(f, true));

            if (filesNotBelongingToZip.Any())
                return Previous.GetCacheDependency(virtualPath, filesNotBelongingToZip, utcStart);
            else
                return new CacheDependency(zipFilePath);
        }

        private ZipEntry GetEntry(string virtualPath, bool isFile)
        {
            string zipPath = Util.ConvertVirtualPathToZipPath(virtualPath, isFile);
            return zipFile.GetEntry(zipPath);
        }

        private bool Exists(string virtualPath, bool isFile)
        {
            EnsureCache();

            bool result = false;
            if (fileOrDirectoryCache.TryGetValue(Util.ConvertVirtualPathToZipPath(virtualPath, isFile), out result))
                return result != isFile;

            return false;
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

                var r = new Regex("^.*/");
                foreach (var entryFileName in zipFile.OfType<ZipEntry>().Select(ze => ze.Name))
                {
                    temp[entryFileName] = false;
                    temp[r.Match(entryFileName).Value] = true;
                }
                fileOrDirectoryCache = temp;
            }
        }

        private string ToRelative(string virtualPath)
        {
            return Url.ToRelative(virtualPath);
        }
    }
}
