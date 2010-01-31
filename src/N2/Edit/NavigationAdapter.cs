using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Engine;
using N2.Collections;
using N2.Web;
using N2.Edit.FileSystem;
using N2.Workflow;

namespace N2.Edit
{
	[Adapts(typeof(ContentItem))]
	public class NavigationAdapter : AbstractContentAdapter
	{
		IEditManager editManager;
		IHost host;
		IFileSystem fileSystem;

		public IFileSystem FileSystem
		{
			get { return fileSystem ?? Engine.Resolve<IFileSystem>(); }
			set { fileSystem = value; }
		}

		public IHost Host
		{
			get { return host ?? Engine.Resolve<IHost>(); }
			set { host = value; }
		}

		public IEditManager EditManager
		{
			get { return editManager ?? Engine.EditManager; }
			set { editManager = value; }
		}

		/// <summary>Gets the filter used to filter child pages.</summary>
		/// <param name="user">The user to filter pages for.</param>
		/// <returns>An item filter used when filtering children to display.</returns>
		public virtual ItemFilter GetChildFilter(System.Security.Principal.IPrincipal user)
		{
			return EditManager.GetEditorFilter(user);
		}

		public virtual IEnumerable<DirectoryData> GetUploadDirectories(ContentItem startPage)
		{
			if (!host.IsStartPage(startPage))
				yield break;

			Site site = host.GetSite(startPage);
			foreach (string uploadFolder in site.UploadFolders)
			{
				yield return FileSystem.GetDirectory(uploadFolder);
			}
			foreach (string uploadFolder in EditManager.UploadFolders)
			{
				yield return FileSystem.GetDirectory(uploadFolder);
			}
		}

		/// <summary>Gets the children of the given item for the given user interface.</summary>
		/// <param name="item">The item whose children to get.</param>
		/// <param name="userInterface">The interface where the children are displayed.</param>
		/// <returns>An enumeration of the children.</returns>
		public virtual IEnumerable<ContentItem> GetChildren(ContentItem parent, string userInterface)
		{
			return parent.GetChildren();
		}
	}
}
