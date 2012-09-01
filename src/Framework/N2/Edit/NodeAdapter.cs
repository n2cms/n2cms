using System;
using System.Linq;
using System.Collections.Generic;
using N2.Collections;
using N2.Edit.FileSystem;
using N2.Edit.Workflow;
using N2.Engine;
using N2.Security;
using N2.Web;
using N2.Edit.Settings;
using N2.Definitions;
using N2.Persistence.Sources;
using System.Text;
using N2.Engine.Globalization;

namespace N2.Edit
{
	[Adapts(typeof(ContentItem))]
	public class NodeAdapter : AbstractContentAdapter
	{
		private IEditUrlManager editUrlManager;
		private IWebContext webContext;
		private IHost host;
		private IFileSystem fileSystem;
		private VirtualNodeFactory nodeFactory;
		private ISecurityManager security;
		private NavigationSettings settings;
		private ContentSource sources;
		private IDefinitionManager definitions;
		private ILanguageGateway languages;

		public NavigationSettings Settings
		{
			get { return settings ?? engine.Resolve<NavigationSettings>(); }
			set { settings = value; }
		}

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

		public ContentSource Sources
		{
			get { return sources ?? engine.Resolve<ContentSource>(); }
			set { sources = value; }
		}

		public IDefinitionManager Definitions
		{
			get { return definitions ?? engine.Resolve<IDefinitionManager>(); }
			set { definitions = value; }
		}

		public ILanguageGateway Languages
		{
			get { return languages ?? engine.Resolve<ILanguageGateway>(); }
			set { languages = value; }
		}


		/// <summary>Gets the node representation used to build the tree hierarchy in the management UI.</summary>
		/// <param name="item">The item to link to.</param>
		/// <returns>Tree node data.</returns>
		public virtual TreeNode GetTreeNode(ContentItem item)
		{
			return new TreeNode
			{
				IconUrl = GetIconUrl(item),
				Title = item.Title,
				MetaInforation = GetMetaInformation(item),
				ToolTip = "#" + item.ID + ": " +  Definitions.GetDefinition(item).Title,
				CssClass = GetClassName(item),
				PreviewUrl = GetPreviewUrl(item),
				MaximumPermission = GetMaximumPermission(item),
			};
		}

		protected virtual IEnumerable<KeyValuePair<string, string>> GetMetaInformation(ContentItem item)
		{
			if (Languages.IsLanguageRoot(item))
				yield return new KeyValuePair<string, string>("language", Languages.GetLanguage(item).LanguageCode);
			if(!item.IsPage)
				yield return new KeyValuePair<string, string>("zone", item.ZoneName);
			if (Host.IsStartPage(item))
				yield return new KeyValuePair<string, string>("authority", string.IsNullOrEmpty(Host.GetSite(item).Authority) ? "*" : Host.GetSite(item).Authority);
		}

		private string GetClassName(ContentItem item)
		{
			StringBuilder className = new StringBuilder();

			if (!item.Published.HasValue || item.Published > DateTime.Now)
				className.Append("unpublished ");
			else if (item.Published > DateTime.Now.AddDays(-1))
				className.Append("day ");
			else if (item.Published > DateTime.Now.AddDays(-7))
				className.Append("week ");
			else if (item.Published > DateTime.Now.AddMonths(-1))
				className.Append("month ");

			if (item.Expires.HasValue && item.Expires <= DateTime.Now)
				className.Append("expired ");

			if (!item.Visible)
				className.Append("invisible ");

			if (item.AlteredPermissions != Permission.None && item.AuthorizedRoles != null && item.AuthorizedRoles.Count > 0)
				className.Append("locked ");

			return className.ToString();
		}

		public virtual IEnumerable<DirectoryData> GetUploadDirectories(Site site)
		{
			foreach (var uploadFolder in site.UploadFolders.Where(uf => uf.Readers.Authorizes(WebContext.User, null, Permission.Read)))
			{
				yield return FileSystem.GetDirectoryOrVirtual(uploadFolder.Path);
			}
		}

		/// <summary>Gets the children of the given item for the given user interface.</summary>
		/// <param name="parent">The item whose children to get.</param>
		/// <param name="userInterface">The interface where the children are displayed.</param>
		/// <returns>An enumeration of the children.</returns>
		public virtual IEnumerable<ContentItem> GetChildren(ContentItem parent, string userInterface)
		{
			IEnumerable<ContentItem> children = GetNodeChildren(parent, userInterface);

			foreach (var child in children)
				yield return child;

			if (Interfaces.Managing == userInterface)
			{
				foreach (var child in NodeFactory.GetChildren(parent.Path))
				{
					yield return child;
				}
			}
		}

		protected virtual IEnumerable<ContentItem> GetNodeChildren(ContentItem parent)
		{
			return GetNodeChildren(parent, Interfaces.Viewing);
		}

		protected virtual IEnumerable<ContentItem> GetNodeChildren(ContentItem parent, string userInterface)
		{
			if (parent is IActiveChildren)
				return ((IActiveChildren)parent).GetChildren(new AccessFilter(WebContext.User, Security));

			var query = new Query { Parent = parent, Interface = userInterface };
			if (!Settings.DisplayDataItems)
				query.OnlyPages = true;
			return Sources.GetChildren(query);
		}

		/// <summary>Returns true when an item has children.</summary>
		/// <param name="filter">The filter to apply.</param>
		/// <param name="parent">The item whose childrens existence is to be determined.</param>
		/// <returns>True when there are children.</returns>
		public virtual bool HasChildren(ContentItem parent, ItemFilter filter)
		{
			return Sources.HasChildren(new Query { Parent = parent, Filter = filter, Interface = Interfaces.Managing });
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
		public virtual string GetIconUrl(ContentItem item)
		{
			return Url.ResolveTokens(item.IconUrl);
		}

		/// <summary>Gets the permissions for the logged in user towards an item.</summary>
		/// <param name="item">The item for which permissions should be retrieved.</param>
		/// <returns>A permission flag.</returns>
		public virtual Permission GetMaximumPermission(ContentItem item)
		{
			return PermissionMap.GetMaximumPermission(Security.GetPermissions(WebContext.User, item));
		}
	}
}
