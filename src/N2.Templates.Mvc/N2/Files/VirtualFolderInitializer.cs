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

namespace N2.Management.Files
{
	[Service]
	public class VirtualFolderInitializer : IAutoStart
	{
		IHost host;
		IPersister persister;
		IFileSystem fs;
		VirtualNodeFactory virtualNodes;
		List<string> uploadFolders = new List<string>();

		public VirtualFolderInitializer(IHost host, IPersister persister, IFileSystem fs, VirtualNodeFactory virtualNodes, EditSection editConfig)
		{
			this.host = host;
			this.persister = persister;
			this.fs = fs;
			this.virtualNodes = virtualNodes;

			foreach (FolderElement folder in editConfig.UploadFolders)
			{
				uploadFolders.Add(folder.Path);
			}
		}

		public ContentItem CreateFolder(string path, string remainingPath)
		{
			var dd = fs.GetDirectory(path);
			return new Directory(dd, null);
		}

		void host_SitesChanged(object sender, SitesChangedEventArgs e)
		{
			UnregisterUploadFolders(e.PreviousDefault, e.PreviousSites);
			RegisterUploadFolders(e.CurrentDefault, e.CurrentSites);
		}

		private void RegisterUploadFolders(Site site, IList<Site> sites)
		{
			Add(site);
			foreach (var s in sites)
			{
				Add(s);
			}
		}

		private void Add(Site site)
		{
			ContentItem item = persister.Get(site.StartPageID);
			if (item == null)
				return;

			foreach (string folder in uploadFolders)
			{
				Register(site, item.Path, folder);
			}
			foreach (string folder in site.UploadFolders)
			{
				Register(site, item.Path, folder);
			}
		}

		private void Register(Site site, string itemPath, string folder)
		{
			var dd = fs.GetDirectory(folder);
			if (dd == null)
				return;

			virtualNodes.Register(new FunctionalNodeProvider(itemPath + folder.TrimStart('~', '/'),
				(remainingPath) => 
				{
					var dir = new Directory(dd, persister.Get(site.StartPageID));
					return string.IsNullOrEmpty(remainingPath) ? dir : dir.GetChild(remainingPath);
				}));
		}

		private void UnregisterUploadFolders(Site site, IList<Site> sites)
		{
			Remove(site);
			foreach (var s in sites)
			{
				Add(s);
			}
		}


		private void Remove(Site site)
		{
			ContentItem item = persister.Get(site.StartPageID);
			if (item == null)
				return;

			foreach (string folder in uploadFolders)
			{
				virtualNodes.Unregister(item.Path + folder.TrimStart('~', '/'));
			}
			foreach (string folder in site.UploadFolders)
			{
				virtualNodes.Unregister(item.Path + folder.TrimStart('~', '/'));
			}
		}

		#region IAutoStart Members

		public void Start()
		{
			RegisterUploadFolders(host.DefaultSite, host.Sites);
			host.SitesChanged += host_SitesChanged;
		}

		public void Stop()
		{
			UnregisterUploadFolders(host.DefaultSite, host.Sites);
			host.SitesChanged -= host_SitesChanged;
		}

		#endregion
	}
}
