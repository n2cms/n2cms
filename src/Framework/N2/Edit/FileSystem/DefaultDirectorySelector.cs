using System;
using System.Collections.Generic;
using N2.Configuration;
using N2.Engine;
using N2.Web;

namespace N2.Edit.FileSystem
{
    /// <summary>
    /// Finds the default folder associated with an item.
    /// </summary>
    [Service(typeof(IDefaultDirectory))]
    public class DefaultDirectorySelector : IDefaultDirectory
    {
        readonly IHost host;
        DefaultDirectoryMode mode;
        string defaultFolderPath;
        readonly List<string> uploadFolders;
        
        public DefaultDirectorySelector(IHost host, EditSection config)
        {
            this.host = host;
            mode = config.DefaultDirectory.Mode;
            defaultFolderPath = config.DefaultDirectory.RootPath;
            uploadFolders = new List<string>(config.UploadFolders.Folders);
        }

        #region IDefaultFolderFinder Members

        public string GetDefaultDirectory(ContentItem item)
        {
            return GetFromItem(item)
                   ?? GetShadowFolder(item)
                      ?? GetFolderRoot();
        }

        #endregion

        private string GetFromItem(ContentItem item)
        {
            if (item is IDocumentBaseSource)
                return (item as IDocumentBaseSource).BaseUrl;
            return null;
        }

        private string GetShadowFolder(ContentItem item)
        {
            if (mode == DefaultDirectoryMode.UploadFolder)
                return null;

            if (mode == DefaultDirectoryMode.NodeName)
                return GetFolderRoot() + item.Name + "/";

            if (item.VersionOf.HasValue)
                item = item.VersionOf;

            string segments = null;
            ContentItem current = mode == DefaultDirectoryMode.RecursiveNamesFromParent ? item.Parent : item;

            for (; current != null && current.Parent != null && !host.IsStartPage(current); current = current.Parent)
            {
                if (mode == DefaultDirectoryMode.TopNodeName)
                    segments = current.Name + "/";
                else
                    segments = current.Name + "/" + segments;
            }
            return GetFolderRoot() + segments;
        }

        private string GetFolderRoot()
        {
            if (!string.IsNullOrEmpty(defaultFolderPath))
                return defaultFolderPath;

            foreach (var folder in host.CurrentSite.UploadFolders)
                return folder.Path;
            foreach (string folder in uploadFolders)
                return folder;

            throw new InvalidOperationException("No upload folder found. Please check your configuration.");
        }
    }
}
