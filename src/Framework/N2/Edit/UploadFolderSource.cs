using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Web;
using N2.Configuration;
using N2.Engine;

namespace N2.Edit
{
	/// <summary>
	/// Used to retrieve upload folders for a certain site. This class is used to determine available folders.
	/// </summary>
	[Service]
	public class UploadFolderSource
	{
		private IHost host;
		private string[] globalFolders;

		public UploadFolderSource(IHost host, EditSection config)
		{
			this.host = host;
			this.globalFolders = config.UploadFolders.Select(u => u.Path).ToArray();
		}

		public IEnumerable<string> GetUploadFoldersForCurrentSite()
		{
			return GetUploadFolders(host.CurrentSite);
		}

		public IEnumerable<string> GetUploadFoldersForAllSites()
		{
			foreach (var folder in globalFolders)
				yield return folder;
			foreach (var folder in host.DefaultSite.UploadFolders)
				yield return folder;
			foreach (var folder in host.Sites.SelectMany(s => s.UploadFolders))
				yield return folder;
		}

		public virtual IEnumerable<string> GetUploadFolders(Site site)
		{
			foreach (var folder in globalFolders)
				yield return folder;
			foreach (var folder in site.UploadFolders)
				yield return folder;
		}
	}
}
