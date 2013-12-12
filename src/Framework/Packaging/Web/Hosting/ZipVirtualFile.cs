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
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace SharpZipLib.Web.VirtualPathProvider
{
    class ZipVirtualFile : VirtualFile
    {
        static object lockObject = new object();

        ZipFile _zipFile;

        public ZipVirtualFile(String virtualPath, ZipFile zipFile)
            : base(virtualPath)
        {
            _zipFile = zipFile;
        }

        public override System.IO.Stream Open()
        {
            ZipEntry entry = _zipFile.GetEntry(Util.ConvertVirtualPathToZipPath(base.VirtualPath, true));
            var buffer = new byte[entry.Size];
            lock (lockObject)
            {
                _zipFile.GetInputStream(entry).Read(buffer, 0, buffer.Length);
                //entry.ZipFileIndex.OpenReader().Read(buffer, 0, buffer.Length);
            }
            var ms = new MemoryStream(buffer);
            return ms;
        }
    }
}
