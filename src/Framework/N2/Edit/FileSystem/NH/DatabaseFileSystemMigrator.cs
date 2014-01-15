using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using N2.Configuration;
using N2.Web;

namespace N2.Edit.FileSystem.NH
{
    /// <summary>
    /// Moves files into the database file system.
    /// </summary>
    public class DatabaseFileSystemMigrator
    {
        private string returnValue = "";
        private IFileSystem fs;
        private UploadFolderSource folderSource;
        private IWebContext webContext;

        public DatabaseFileSystemMigrator(IFileSystem fs, UploadFolderSource folderSource, IWebContext webContext)
        {
            this.fs = fs;
            this.folderSource = folderSource;
            this.webContext = webContext;
        }

        public string CopyToDb()
        {
            if (fs.GetType() != typeof(DatabaseFileSystem))
                throw new Exception("Database filesystem not configured");

            var uploadFolders = folderSource.GetUploadFoldersForAllSites();
            foreach (var uploadFolder in uploadFolders)
            {
                CopyFilesInDirectoryRecursively(webContext.MapPath(uploadFolder.Path));
            }

            return returnValue;
        }

        private void CopyFilesInDirectoryRecursively(string dirPath)
        {
            var files = System.IO.Directory.GetFiles(dirPath);
            
            foreach (var file in files)
            {
                var relativeFile = FullToRelative(file);
                
                if (fs.FileExists(relativeFile)) continue;

                returnValue += "<br />" + relativeFile;
                using (var fileStream = System.IO.File.OpenRead(file))
                {
                    fs.WriteFile(relativeFile, fileStream);
                    fileStream.Close();
                }
            }

            var subdirs = System.IO.Directory.GetDirectories(dirPath);
            foreach (var subdir in subdirs)
            {
                var relativeDir = FullToRelative(subdir);
                if (!fs.DirectoryExists(relativeDir))
                {
                    returnValue += "<br />" + relativeDir;
                    fs.CreateDirectory(relativeDir);
                }
                CopyFilesInDirectoryRecursively(subdir);
            }
        }

        private string FullToRelative(string physicalPath)
        {
            var basePath = HttpContext.Current.Server.MapPath("~/");
            return physicalPath.Replace(basePath, "~/").Replace(@"\", "/");
        }
    }
}
