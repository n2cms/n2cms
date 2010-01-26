using System;
using N2.Collections;
using N2.Edit.FileSystem.Items;
using N2.Web;

namespace N2.Edit.Navigation
{
	public partial class Tree : NavigationPage
	{
		protected override void OnInit(EventArgs e)
		{
			if (Request["location"] == "files" || Request["location"] == "filesselection")
			{
				HierarchyNode<ContentItem> root;
				
				IHost host = Engine.Resolve<IHost>();
				if (host.Sites.Count > 1)
				{
					root = new HierarchyNode<ContentItem>(Engine.Persister.Get(host.DefaultSite.RootItemID));
					foreach (var site in host.Sites)
					{
						root.Children.Add(CreateSiteFilesNode(site));
					}
				}
				else
				{
					root = CreateSiteFilesNode(host.CurrentSite);
				}

				siteTreeView.Nodes = root;
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

		private HierarchyNode<ContentItem> CreateSiteFilesNode(Site site)
		{
			var siteNode = Engine.Persister.Get(site.StartPageID);
			HierarchyNode<ContentItem> node = new HierarchyNode<ContentItem>(siteNode);
			foreach (string uploadFolder in site.UploadFolders)
			{
				node.Children.Add(CreateDirectory(uploadFolder, siteNode));
			}
			foreach (string uploadFolder in Engine.EditManager.UploadFolders)
			{
				node.Children.Add(CreateDirectory(uploadFolder, siteNode));
			}
			return node;
		}

		private HierarchyNode<ContentItem> CreateDirectory(string uploadFolder, ContentItem siteNode)
		{
			var dir = Engine.Resolve<N2.Edit.FileSystem.IFileSystem>().GetDirectory(uploadFolder);
			return new HierarchyNode<ContentItem>(new Directory(dir, siteNode));
		}
	}
}
