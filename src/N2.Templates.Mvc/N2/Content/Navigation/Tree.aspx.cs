using System;
using N2.Collections;
using N2.Edit.FileSystem.Items;
using N2.Web;
using N2.Engine;
using N2.Edit.FileSystem;

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
				if (host.Sites.Count > 0)
				{
					root = new HierarchyNode<ContentItem>(Engine.Persister.Get(host.DefaultSite.RootItemID));

					root.Children.Add(CreateSiteFilesNode(host.DefaultSite));
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
			foreach (DirectoryData dir in Engine.Resolve<IContentAdapterProvider>()
				.ResolveAdapter<NodeAdapter>(siteNode.GetType())
				.GetUploadDirectories(siteNode))
			{
				node.Children.Add(new HierarchyNode<ContentItem>(new Directory(dir, siteNode)));
			}

			return node;
		}
	}
}
