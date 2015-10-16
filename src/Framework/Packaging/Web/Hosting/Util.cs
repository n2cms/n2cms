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
using System.Web;

namespace SharpZipLib.Web.VirtualPathProvider
{
    internal static class Util
    {
        internal static string ConvertVirtualPathToZipPath (String virtualPath, bool isFile)
        {
            if (virtualPath[0] == '~')
            {
                if (!isFile)
                    return virtualPath.Substring(2).TrimEnd('/') + "/";
                else
                    return virtualPath.Substring(2);
            }
            else if (virtualPath[0] == '/')
            {
                virtualPath = ToAppRelative(virtualPath);
                if (!isFile)
                    return virtualPath.Substring(2).TrimEnd('/') + "/";
                else
                    return virtualPath.Substring(2);
            }
            else
                return virtualPath;
        }

        private static string ToAppRelative(String virtualPath)
        {
            if(HttpContext.Current == null || !virtualPath.StartsWith(HttpRuntime.AppDomainAppVirtualPath))
                return "~" + virtualPath;

            return VirtualPathUtility.ToAppRelative(virtualPath);
        }

        internal static string ConvertZipPathToVirtualPath (String zipPath)
        {
            return "/" + zipPath;
        }
    }

    internal enum VirtualPathType
    {
        Files, Directories, All
    }
}
