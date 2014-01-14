using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Web;
using N2.Configuration;
using N2.Engine;
using System.Collections.Specialized;
using N2.Security;

namespace N2.Edit
{
    public class FileSystemRoot
    {
        public FileSystemRoot(string path, Site site)
        {
            Path = path;
            Readers = new PermissionMap(Permission.Read, new[] { "Administrators", "Editors", "Writers" }, new[] { "admin" });
            Writers = new PermissionMap(Permission.Write, new[] { "Administrators", "Editors", "Writers" }, new[] { "admin" });
            Site = site;
        }

        public FileSystemRoot(FolderElement folder, Site site)
        {
            Path = FixPath(folder.Path);
            if (!string.IsNullOrEmpty(folder.Title))
                Title = folder.Title;
            Readers = folder.Readers.ToPermissionMap(Permission.Read, new[] { "Administrators", "Editors", "Writers" }, new[] { "admin" });
            Writers = folder.Writers.ToPermissionMap(Permission.Write, new[] { "Administrators", "Editors" }, new[] { "admin" });
            Site = site;
            if (!string.IsNullOrEmpty(folder.UrlPrefix))
                UrlPrefix = folder.UrlPrefix.TrimEnd('/');
        }

        public Site Site { get; set; }
        public string Path { get; set; }
        public string Title { get; set; }
        public bool IsGlobal { get; set; }
        public PermissionMap Readers { get; set; }
        public PermissionMap Writers { get; set; }
        public string UrlPrefix { get; set; }

        private static string FixPath(string path)
        {
            if (!path.EndsWith("/"))
                path = path + "/";
            if (path.StartsWith("/"))
                path = "~" + path;
            else if (!path.StartsWith("~"))
                path = "~/" + path;
            return path;
        }

        public static implicit operator FileSystemRoot(string path)
        {
            return new FileSystemRoot(path, null);
        }

        public int GetParentID()
        {
            return IsGlobal ? Site.RootItemID : Site.StartPageID;
        }

        public string GetName()
        {
            return Path.Trim('~', '/');
        }
    }

    /// <summary>
    /// Used to retrieve upload folders for a certain site. This class is used to determine available folders.
    /// </summary>
    [Service]
    public class UploadFolderSource
    {
        private IHost host;
        private FileSystemRoot[] globalFolders;

        public UploadFolderSource(IHost host, EditSection config)
        {
            this.host = host;
            this.globalFolders = config.UploadFolders
                .AllElements.Select(u => new FileSystemRoot(u, host.DefaultSite) { IsGlobal = true }).ToArray();
        }

        public IEnumerable<FileSystemRoot> GetUploadFoldersForCurrentSite()
        {
            return GetUploadFolders(host.CurrentSite);
        }

        public virtual IEnumerable<FileSystemRoot> GetUploadFoldersForAllSites()
        {
            foreach (var folder in globalFolders)
                yield return folder;
            foreach (var folder in host.DefaultSite.UploadFolders)
                yield return folder;
            foreach (var folder in host.Sites.SelectMany(s => s.UploadFolders.Select(f =>
            {
                if (f.Site == null)
                    f.Site = s;
                return f;
            })))
            {
                yield return folder;
            }
        }

        public virtual IEnumerable<FileSystemRoot> GetUploadFolders(Site site)
        {
            foreach (var folder in globalFolders)
                yield return folder;
            foreach (var folder in site.UploadFolders)
                yield return folder;
        }
    }
}
