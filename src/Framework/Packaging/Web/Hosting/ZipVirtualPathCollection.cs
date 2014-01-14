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
using System.Collections;
using ICSharpCode.SharpZipLib.Zip;
//using System.Collections.Generic;
//using System.Text;

namespace SharpZipLib.Web.VirtualPathProvider
{
    class ZipVirtualPathCollection : MarshalByRefObject, IEnumerable
    {
        ZipFile _zipFile;
        ArrayList _paths;
        String _virtualPath;
        VirtualPathType _requestType;

        public ZipVirtualPathCollection (String virtualPath, VirtualPathType requestType, ZipFile zipFile) {
            _paths = new ArrayList();
            _virtualPath = virtualPath;
            _requestType = requestType;
            _zipFile = zipFile;

            PerformEnumeration ();
        }

        private void PerformEnumeration ()
        {
            String zipPath = Util.ConvertVirtualPathToZipPath (_virtualPath, false);

            if (zipPath[zipPath.Length - 1] != '/') {
                ZipEntry entry = _zipFile.GetEntry(zipPath);
                if (entry != null)
                    _paths.Add (new ZipVirtualFile (zipPath, _zipFile));
                return;
            }
            else {
                foreach (ZipEntry entry in _zipFile) {
                    Console.WriteLine (entry.Name);
                    if (entry.Name == zipPath)
                        continue;
                    if (entry.Name.StartsWith(zipPath))
                    {
                        // if we're looking for files and current entry is a directory, skip it
                        if (_requestType == VirtualPathType.Files && entry.IsDirectory)
                            continue;
                        // if we're looking for directories and current entry its not one, skip it
                        if (_requestType == VirtualPathType.Directories && !entry.IsDirectory)
                            continue;

                        int pos = entry.Name.IndexOf('/', zipPath.Length);
                        if (pos != -1) {
                            if (entry.Name.Length > pos + 1)
                                continue;
                        }
                        //    continue;
                        if (entry.IsDirectory)
                            _paths.Add(new ZipVirtualDirectory(Util.ConvertZipPathToVirtualPath(entry.Name), _zipFile));
                        else
                            _paths.Add(new ZipVirtualFile(Util.ConvertZipPathToVirtualPath(entry.Name), _zipFile));
                    }
                }
            }
        }

        public override object InitializeLifetimeService () {
            return null;
        }

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator () {
            return _paths.GetEnumerator ();
        }

        #endregion
    }
}
