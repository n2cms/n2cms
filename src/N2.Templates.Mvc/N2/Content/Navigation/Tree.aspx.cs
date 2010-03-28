using System;
using N2.Collections;
using N2.Edit.FileSystem.Items;
using N2.Web;
using N2.Engine;
using N2.Edit.FileSystem;
using N2.Configuration;

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

				var fs = Engine.Resolve<IFileSystem>();
				foreach (string uploadFolder in Engine.EditManager.UploadFolders)
				{
					var dd = fs.GetDirectory(uploadFolder);
					root.Children.Add(new HierarchyNode<ContentItem>(new Directory(fs, dd, root.Current)));
				}

				AddSiteFilesNodes(fs, root, host.DefaultSite);
				foreach (var site in host.Sites)
				{
					AddSiteFilesNodes(fs, root, site);
				}
				
				siteTreeView.Nodes = root;
				//TODO:siteTreeView.SelectedItem = Selection.SelectedItem;
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

		private void AddSiteFilesNodes(IFileSystem fs, HierarchyNode<ContentItem> parent, Site site)
		{
			var siteNode = Engine.Persister.Get(site.StartPageID);

			HierarchyNode<ContentItem> node = null;
			foreach (DirectoryData dir in Engine.Resolve<IContentAdapterProvider>()
				.ResolveAdapter<NodeAdapter>(siteNode.GetType())
				.GetUploadDirectories(site))
			{
				if(node == null)
					node = new HierarchyNode<ContentItem>(siteNode);
				node.Children.Add(new HierarchyNode<ContentItem>(new Directory(fs, dir, siteNode)));
			}

			if (node != null)
				parent.Children.Add(node);
		}
	}
}
