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
		FileSystemFolderCollection folders;
		IHost host;
		IPersister persister;
		VirtualNodeFactory virtualNodes;
		FolderNodeProvider nodeProvider;
		ConnectionMonitor monitor;

		/// <summary>
		/// Initializes a new instance of the <see cref="VirtualFolderInitializer"/> class.
		/// </summary>
		/// <param name="host">The host.</param>
		/// <param name="persister">The persister.</param>
		/// <param name="fs">The fs.</param>
		/// <param name="virtualNodes">The virtual nodes.</param>
		/// <param name="editConfig">The edit config.</param>
		public VirtualFolderInitializer(IHost host, IPersister persister, IFileSystem fs, VirtualNodeFactory virtualNodes, ConnectionMonitor monitor, EditSection editConfig, ImageSizeCache imageSizes, FolderNodeProvider nodeProvider)
		{
			this.host = host;
			this.persister = persister;
			this.virtualNodes = virtualNodes;
			this.monitor = monitor;
			this.folders = editConfig.UploadFolders;
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
			nodeProvider.UploadFolderPaths = GetUploadFolderPaths();
			//virtualNodes.Register(nodeProvider);
		}

		void host_SitesChanged(object sender, SitesChangedEventArgs e)
		{
			nodeProvider.UploadFolderPaths = GetUploadFolderPaths();
		}

		private FolderPair[] GetUploadFolderPaths()
		{
			var paths = new List<FolderPair>();

			// configured folders to the root node
			foreach (FolderElement folder in folders)
			{
				var root = persister.Get(host.DefaultSite.RootItemID);
				var pair = new FolderPair(root.ID, root.Path, folder.Path.TrimStart('~'), folder.Path);
				paths.Add(pair);
			}
			// site-upload folders to their respective nodes
			paths.AddRange(UploadFoldersForSite(host.DefaultSite));
			foreach (var site in host.Sites)
			{
				paths.AddRange(UploadFoldersForSite(site));
			}

			return paths.ToArray();
		}

		private IEnumerable<FolderPair> UploadFoldersForSite(Site site)
		{
			ContentItem item = persister.Get(site.StartPageID);
			if (item == null)
				yield break;

			string itemPath = item.Path;
			foreach (string path in site.UploadFolders)
			{
				yield return new FolderPair(item.ID, item.Path, itemPath + path.TrimStart('~', '/'), path);
			}
		}
	}
}
