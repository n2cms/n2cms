using System;
using System.Collections.Generic;
using System.Text;
using N2.Engine;
using N2.Plugin;
using N2.Persistence;
using N2.Web;
using N2.Configuration;
using N2.Edit.FileSystem;
using N2.Edit;
using N2.Edit.FileSystem.Items;
using N2.Collections;
using System.Diagnostics;
using N2.Edit.Installation;

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
		IFileSystem fs;
		VirtualNodeFactory virtualNodes;
		FolderNodeProvider nodeProvider;
		DatabaseStatusCache dbStatus;

		/// <summary>
		/// Initializes a new instance of the <see cref="VirtualFolderInitializer"/> class.
		/// </summary>
		/// <param name="host">The host.</param>
		/// <param name="persister">The persister.</param>
		/// <param name="fs">The fs.</param>
		/// <param name="virtualNodes">The virtual nodes.</param>
		/// <param name="editConfig">The edit config.</param>
		public VirtualFolderInitializer(IHost host, IPersister persister, IFileSystem fs, VirtualNodeFactory virtualNodes, DatabaseStatusCache dbStatus, EditSection editConfig)
		{
			this.host = host;
			this.persister = persister;
			this.fs = fs;
			this.virtualNodes = virtualNodes;
			this.dbStatus = dbStatus;
			this.folders = editConfig.UploadFolders;

			nodeProvider = new FolderNodeProvider(fs, persister);
		}

		#region IAutoStart Members

		public void Start()
		{
			if (dbStatus.GetStatus() >= SystemStatusLevel.UpAndRunning)
			{
				nodeProvider.UploadFolderPaths = GetUploadFolderPaths();
				virtualNodes.Register(nodeProvider);
				host.SitesChanged += host_SitesChanged;
			}
			else
				dbStatus.DatabaseStatusChanged += dbStatus_DatabaseStatusChanged;
		}

		void dbStatus_DatabaseStatusChanged(object sender, EventArgs e)
		{
			dbStatus.DatabaseStatusChanged -= dbStatus_DatabaseStatusChanged;
			Start();
		}

		public void Stop()
		{
			host.SitesChanged -= host_SitesChanged;
			virtualNodes.Unregister(nodeProvider);
		}

		#endregion

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

		/// <summary>
		/// A custom <see cref="INodeProvider" /> that enumerates the FileSystem as special ContentItem instances
		/// </summary>
		class FolderNodeProvider : INodeProvider
		{
			IFileSystem fs;
			IPersister persister;

			public FolderPair[] UploadFolderPaths { get; set; }

			public FolderNodeProvider(IFileSystem fs, IPersister persister)
			{
				UploadFolderPaths = new FolderPair[0];
				this.fs = fs;
				this.persister = persister;
			}


			#region INodeProvider Members

			public ContentItem Get(string path)
			{
				foreach (var pair in UploadFolderPaths)
				{
					if (path.StartsWith(pair.Path, StringComparison.InvariantCultureIgnoreCase))
					{
						var dir = CreateDirectory(pair);

						string remaining = path.Substring(pair.Path.Length);
						if (string.IsNullOrEmpty(remaining))
							return dir;
						return dir.GetChild(remaining);
					}
				}

				return null;
			}

			public IEnumerable<ContentItem> GetChildren(string path)
			{
				foreach (var pair in UploadFolderPaths)
				{
					if (pair.ParentPath.Equals(path, StringComparison.InvariantCultureIgnoreCase))
					{
						var dd = fs.GetDirectory(pair.FolderPath);
						var dir = CreateDirectory(pair);
						yield return dir;
					}
				}
			}

			private Directory CreateDirectory(FolderPair pair)
			{
				var dd = fs.GetDirectory(pair.FolderPath);
				var parent = persister.Get(pair.ParentID);

				var dir = new Directory(dd, parent);
				dir.Set(fs);
				dir.Title = pair.Path.Substring(pair.ParentPath.Length).Trim('/');
				dir.Name = dir.Title;

				return dir;
			}

			#endregion
		}



		struct FolderPair
		{
			public FolderPair(int parentID, string parentPath, string path, string folderPath)
			{
				ParentID = parentID;
				Path = path;
				ParentPath = parentPath;
				FolderPath = folderPath;
			}

			public int ParentID;
			public string Path;
			public string ParentPath;
			public string FolderPath;
		}
	}
}
