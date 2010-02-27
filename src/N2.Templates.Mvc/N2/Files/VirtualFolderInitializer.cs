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

namespace N2.Management.Files
{
	[Service]
	public class VirtualFolderInitializer : IAutoStart
	{
		FileSystemFolderCollection folders;
		IHost host;
		IPersister persister;
		IFileSystem fs;
		VirtualNodeFactory virtualNodes;
		FolderNodeProvider nodeProvider;

		public VirtualFolderInitializer(IHost host, IPersister persister, IFileSystem fs, VirtualNodeFactory virtualNodes, EditSection editConfig)
		{
			this.host = host;
			this.persister = persister;
			this.fs = fs;
			this.virtualNodes = virtualNodes;
			this.folders = editConfig.UploadFolders;

			nodeProvider = new FolderNodeProvider(fs, persister);
		}

		#region IAutoStart Members

		public void Start()
		{
			nodeProvider.UploadFolderPaths = GetUploadFolderPaths();
			virtualNodes.Register(nodeProvider);
			host.SitesChanged += host_SitesChanged;
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
				var pair = new FolderPair(host.DefaultSite.RootItemID, folder.Path.TrimStart('~'), folder.Path);
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
				yield return new FolderPair(item.ID, itemPath + path.TrimStart('~', '/'), path);
			}
		}



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
						var dir = CreateDirectory(pair.FolderPath, pair.ParentID);

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
						yield return new Directory(fs, dd, persister.Get(pair.ParentID));
					}
				}
			}

			private Directory CreateDirectory(string folderPath, int parentID)
			{
				var dd = fs.GetDirectory(folderPath);
				var parent = persister.Get(parentID);

				return new Directory(fs, dd, parent);
			}

			#endregion
		}



		struct FolderPair
		{
			public FolderPair(int parentID, string path, string folderPath)
			{
				ParentID = parentID;
				Path = path;
				ParentPath = N2.Web.Url.RemoveLastSegment(path);
				FolderPath = folderPath;
			}

			public int ParentID;
			public string Path;
			public string ParentPath;
			public string FolderPath;
		}
	}
}
