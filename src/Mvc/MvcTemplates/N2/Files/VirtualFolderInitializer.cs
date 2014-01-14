using System;
using System.Collections.Generic;
using N2.Configuration;
using N2.Edit;
using N2.Edit.FileSystem;
using N2.Edit.FileSystem.Items;
using N2.Edit.Installation;
using N2.Engine;
using N2.Persistence;
using N2.Plugin;
using N2.Web;
using N2.Collections;
using System.Linq;

namespace N2.Management.Files
{
    /// <summary>
    /// Auto-startable service that registers a custom <see cref="INodeProvider" /> within N2's <see cref="VirtualNodeFactory" /> infrastructure.
    /// 
    /// The <see cref="INodeProvider" /> provides the contents of the file manager library, effectively integrating the file manager with N2's page tree.
    /// </summary>
    [Service]
    public class VirtualFolderInitializer : IAutoStart
    {
        IHost host;
        IPersister persister;
        VirtualNodeFactory virtualNodes;
        FolderNodeProvider nodeProvider;
        ConnectionMonitor monitor;
        UploadFolderSource folderSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualFolderInitializer"/> class.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="persister">The persister.</param>
        /// <param name="fs">The fs.</param>
        /// <param name="virtualNodes">The virtual nodes.</param>
        /// <param name="editConfig">The edit config.</param>
        public VirtualFolderInitializer(IHost host, IPersister persister, IFileSystem fs, VirtualNodeFactory virtualNodes, ConnectionMonitor monitor, UploadFolderSource folderSource, FolderNodeProvider nodeProvider)
        {
            this.host = host;
            this.persister = persister;
            this.virtualNodes = virtualNodes;
            this.monitor = monitor;
            this.folderSource = folderSource;
            this.nodeProvider = nodeProvider;
        }

        #region IAutoStart Members

        public void Start()
        {
            monitor.Online += monitor_Online;
            host.SitesChanged += host_SitesChanged;
        }

        public void Stop()
        {
            monitor.Online -= monitor_Online;
            host.SitesChanged -= host_SitesChanged;
        }

        #endregion

        void monitor_Online(object sender, EventArgs e)
        {
            nodeProvider.UploadFolderPaths = GetUploadFolderPaths().ToArray();
        }

        void host_SitesChanged(object sender, SitesChangedEventArgs e)
        {
            nodeProvider.UploadFolderPaths = GetUploadFolderPaths().ToArray();
        }

        protected virtual IEnumerable<FolderReference> GetUploadFolderPaths()
        {
            var paths = new List<FolderReference>();

            if (folderSource == null)
                throw new NullReferenceException("folderSource is null");
                //return new FolderPair[0];
            
            var gpp = new List<FileSystemRoot>(folderSource.GetUploadFoldersForAllSites()); // non-lazy easier to debug :-)

            if (gpp == null)
                throw new NullReferenceException("folderSource.GetUploadFoldersForAllSites() returned null");

            // configured folders to the root node
            foreach (var folder in gpp)
            {
                var parent = persister.Get(folder.GetParentID());
                if (parent == null)
                    break;

                var pair = new FolderReference(parent.ID, parent.Path, parent.Path + folder.GetName() + "/", folder);
                paths.Add(pair);
            }

            return paths;
        }
    }
}
