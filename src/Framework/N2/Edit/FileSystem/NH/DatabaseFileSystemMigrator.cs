using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using N2.Configuration;
using N2.Web;

namespace N2.Edit.FileSystem.NH
{
    public static class DatabaseFileSystemMigrator
    {
        static string returnValue = "";
        static IFileSystem fs = (Context.Current.Resolve<IFileSystem>());

        public static string CopyToDb()
        {
            if (fs.GetType() != typeof(DatabaseFileSystem))
                throw new Exception("Database filesystem not configured");

            var uploadFolders = new EditSection().UploadFolders;
            foreach (var uploadFolder in uploadFolders)
            {
                CopyFilesInDirectoryRecursively(HttpContext.Current.Server.MapPath(uploadFolder.Path));
            }

            return returnValue;
        }

        private static void CopyFilesInDirectoryRecursively(string dirPath)
        {
            var files = System.IO.Directory.GetFiles(dirPath);
            
            foreach (var file in files)
            {
                var relativeFile = FullToRelative(file);
                returnValue += "<br />" + relativeFile;
                using(var fileStream = System.IO.File.OpenRead(file))
                {
                    fs.WriteFile(relativeFile, fileStream);   
                    fileStream.Close();
                }
            }

            var subdirs = System.IO.Directory.GetDirectories(dirPath);
            foreach (var subdir in subdirs)
            {
                var relativeDir = FullToRelative(subdir);
                returnValue += "<br />" + relativeDir;
                if (!fs.DirectoryExists(relativeDir))
                {
                    fs.CreateDirectory(relativeDir);
                }
                CopyFilesInDirectoryRecursively(subdir);
            }
        }

        private static string FullToRelative(string physicalPath)
        {
            var basePath = HttpContext.Current.Server.MapPath("~/");
            return physicalPath.Replace(basePath, "~/").Replace(@"\", "/");
        }
    }
}
