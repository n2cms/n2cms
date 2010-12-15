using System;
using System.Collections.Generic;
using System.Security.Principal;
using N2.Collections;
using N2.Edit.FileSystem;
using N2.Engine;
using N2.Web;
using N2.Edit.Workflow;
using N2.Security;

namespace N2.Edit
{
	[Adapts(typeof(ContentItem))]
	public class NodeAdapter : AbstractContentAdapter
	{
		IEditUrlManager editUrlManager;
		IWebContext webContext;
		IHost host;
		IFileSystem fileSystem;
		VirtualNodeFactory nodeFactory;
		ISecurityManager security;

		public ISecurityManager Security
		{
			get { return security ?? engine.Resolve<ISecurityManager>(); }
			set { security = value; }
		}

		public IWebContext WebContext
		{
			get { return webContext ?? engine.Resolve<IWebContext>(); }
			set { webContext = value; }
		}

		public VirtualNodeFactory NodeFactory
		{
			get { return nodeFactory ?? engine.Resolve<VirtualNodeFactory>(); }
			set { nodeFactory = value; }
		}

		public IFileSystem FileSystem
		{
			get { return fileSystem ?? engine.Resolve<IFileSystem>(); }
			set { fileSystem = value; }
		}

		public IHost Host
		{
			get { return host ?? engine.Resolve<IHost>(); }
			set { host = value; }
		}

		public IEditUrlManager ManagementPaths
		{
			get { return editUrlManager ?? engine.ManagementPaths; }
			set { editUrlManager = value; }
		}

		public virtual IEnumerable<DirectoryData> GetUploadDirectories(Site site)
		{
			foreach (string uploadFolder in site.UploadFolders)
			{
				yield return FileSystem.GetDirectory(uploadFolder);
			}
		}

		/// <summary>Gets the children of the given item for the given user interface.</summary>
		/// <param name="parent">The item whose children to get.</param>
		/// <param name="userInterface">The interface where the children are displayed.</param>
		/// <returns>An enumeration of the children.</returns>
		public virtual IEnumerable<ContentItem> GetChildren(ContentItem parent, string userInterface)
		{
			foreach (var child in parent.GetChildren(new AccessFilter(WebContext.User, Security)))
			{
				yield return child;
			}
			if (Interfaces.Managing == userInterface)
			{
				foreach (var child in NodeFactory.GetChildren(parent.Path))
				{
					yield return child;
				}
			}
		}

		/// <summary>Returns true when an item has children.</summary>
		/// <param name="filter">The filter to apply.</param>
		/// <param name="parent">The item whose childrens existence is to be determined.</param>
		/// <returns>True when there are children.</returns>
		public virtual bool HasChildren(ContentItem parent, ItemFilter filter)
		{
			return parent.GetChildren(filter).Count > 0;
		}

		/// <summary>Gets the url used from the management UI when previewing an item.</summary>
		/// <param name="item">The item to preview.</param>
		/// <returns>An url to preview the item.</returns>
		public virtual string GetPreviewUrl(ContentItem item)
		{
			string url = ManagementPaths.GetPreviewUrl(item);
			url = String.IsNullOrEmpty(url) ? ManagementPaths.ResolveResourceUrl("{ManagementUrl}/Empty.aspx") : url;
			return url;
		}

		/// <summary>Gets the url to the icon representing this item.</summary>
		/// <param name="item">The item whose icon to get.</param>
		/// <returns>An url to an icon.</returns>
		public string GetIconUrl(ContentItem item)
		{
			return ManagementPaths.ResolveResourceUrl(item.IconUrl);
		}
	}
}
