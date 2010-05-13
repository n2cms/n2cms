using System;
using N2.Collections;
using N2.Edit.FileSystem.Items;
using N2.Web;
using N2.Engine;
using N2.Edit.FileSystem;
using N2.Configuration;
using System.Collections.Generic;

namespace N2.Edit.Navigation
{
	[ToolbarPlugin("PAGES", "tree", "Content/default.aspx?selected={selected}", ToolbarArea.Navigation, Targets.Top, "~/N2/Resources/icons/sitemap_color.png", -30,
		ToolTip = "show navigation", 
		GlobalResourceClassName = "Toolbar", SortOrder = -1)]
	public partial class Tree : NavigationPage
	{
		protected override void OnInit(EventArgs e)
		{
			if (Request["location"] == "files" || Request["location"] == "filesselection")
			{
				IHost host = Engine.Resolve<IHost>();
				HierarchyNode<ContentItem> root = new HierarchyNode<ContentItem>(Engine.Persister.Get(host.DefaultSite.RootItemID));

				var selectionTrail = new List<ContentItem>();
				if (Selection.SelectedItem is AbstractNode)
				{
					selectionTrail = new List<ContentItem>(Find.EnumerateParents(Selection.SelectedItem, null, true));
				}

				var fs = Engine.Resolve<IFileSystem>();
				foreach (string uploadFolder in Engine.EditManager.UploadFolders)
				{
					var dd = fs.GetDirectory(uploadFolder);

					var node = CreateDirectoryNode(fs, new Directory(fs, dd, root.Current), root, selectionTrail);
					root.Children.Add(node);
				}

				AddSiteFilesNodes(fs, root, host.DefaultSite, selectionTrail);
				foreach (var site in host.Sites)
				{
					AddSiteFilesNodes(fs, root, site, selectionTrail);
				}

				siteTreeView.Nodes = root;
				siteTreeView.SelectedItem = Selection.SelectedItem;
			}
			else
			{
				var filter = Engine.EditManager.GetEditorFilter(Page.User);
				siteTreeView.Filter = filter;
				siteTreeView.RootNode = RootNode;
				siteTreeView.SelectedItem = Selection.SelectedItem;
			}
			
			siteTreeView.DataBind();

			base.OnInit(e);
		}

		private void AddSiteFilesNodes(IFileSystem fs, HierarchyNode<ContentItem> parent, Site site, List<ContentItem> selectionTrail)
		{
			var siteNode = Engine.Persister.Get(site.StartPageID);

			HierarchyNode<ContentItem> node = null;
			foreach (DirectoryData dd in Engine.Resolve<IContentAdapterProvider>()
				.ResolveAdapter<NodeAdapter>(siteNode.GetType())
				.GetUploadDirectories(site))
			{
				if(node == null)
					node = new HierarchyNode<ContentItem>(siteNode);
				var directoryNode = CreateDirectoryNode(fs, new Directory(fs, dd, parent.Current), node, selectionTrail);
				node.Children.Add(directoryNode);
			}

			if (node != null)
				parent.Children.Add(node);
		}

		private static HierarchyNode<ContentItem> CreateDirectoryNode(IFileSystem fs, ContentItem directory, HierarchyNode<ContentItem> parent, List<ContentItem> selectionTrail)
		{
			var node = new HierarchyNode<ContentItem>(directory);
			ExpandRecursive(fs, node, selectionTrail);

			return node;
		}

		private static void ExpandRecursive(IFileSystem fs, HierarchyNode<ContentItem> parent, List<ContentItem> selectionTrail)
		{
			int index = selectionTrail.FindIndex(ci => string.Equals(ci.Url, parent.Current.Url, StringComparison.InvariantCultureIgnoreCase));
			if (index >= 0)
			{
				foreach (var child in parent.Current.GetChildren(new NullFilter()))
				{
					parent.Children.Add(CreateDirectoryNode(fs, child, parent, selectionTrail));
				}
			}
		}
	}
}
