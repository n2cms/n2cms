using System;
using N2.Collections;
using N2.Edit.FileSystem.Items;
using N2.Web;
using N2.Engine;
using N2.Edit.FileSystem;
using N2.Configuration;
using System.Collections.Generic;
using System.Web.UI.HtmlControls;
using System.Web;

namespace N2.Edit.Navigation
{
	public partial class Tree : NavigationPage
	{
		protected HtmlInputHidden inputLocation;
		protected HtmlInputFile inputFile;
		protected IFileSystem FS;

		protected override void OnPreInit(EventArgs e)
		{
			base.OnPreInit(e);
			FS = Engine.Resolve<IFileSystem>();
		}

		protected override void OnInit(EventArgs e)
		{
			var selected = Selection.SelectedItem;
			if (IsPostBack && !string.IsNullOrEmpty(inputFile.PostedFile.FileName))
			{
				string uploadFolder = Request["inputLocation"];
				if(!IsAvailable(uploadFolder))
					throw new N2Exception("Cannot upload to " + Server.HtmlEncode(uploadFolder));

				string fileName = System.IO.Path.GetFileName(inputFile.PostedFile.FileName);
				string filePath = VirtualPathUtility.Combine(uploadFolder, fileName);
				FS.WriteFile(filePath, inputFile.PostedFile.InputStream);

				ClientScript.RegisterStartupScript(typeof(Tree), "select", "updateOpenerWithUrlAndClose('" + ResolveUrl(filePath) + "');", true);
			}
			else if (Request["location"] == "files" || Request["location"] == "filesselection")
			{
				IHost host = Engine.Resolve<IHost>();
				HierarchyNode<ContentItem> root = new HierarchyNode<ContentItem>(Engine.Persister.Get(host.DefaultSite.RootItemID));

				var selectionTrail = new List<ContentItem>();
				if (selected is AbstractNode)
				{
					selectionTrail = new List<ContentItem>(Find.EnumerateParents(selected, null, true));
				}
				else
				{
					TrySelectingPrevious(ref selected, ref selectionTrail);
				}

				foreach (string uploadFolder in Engine.EditManager.UploadFolders)
				{
					var dd = FS.GetDirectory(uploadFolder);

					var dir = new Directory(dd, root.Current);
					dir.Set(FS);
					var node = CreateDirectoryNode(FS, dir, root, selectionTrail);
					root.Children.Add(node);
				}

				AddSiteFilesNodes(root, host.DefaultSite, selectionTrail);
				foreach (var site in host.Sites)
				{
					AddSiteFilesNodes(root, site, selectionTrail);
				}

				siteTreeView.Nodes = root;
				siteTreeView.SelectedItem = selected;
			}
			else
			{
				var filter = Engine.EditManager.GetEditorFilter(Page.User);
				siteTreeView.Filter = filter;
				siteTreeView.RootNode = Engine.Resolve<Navigator>().Navigate(Request["root"] ?? "/");
				siteTreeView.SelectedItem = selected;
			}
			
			siteTreeView.DataBind();

			base.OnInit(e);
		}

		private void TrySelectingPrevious(ref ContentItem selected, ref List<ContentItem> selectionTrail)
		{
			var cookie = Request.Cookies["lastSelection"];
			if (cookie == null || string.IsNullOrEmpty(cookie.Value))
				return;

			string recenSelectionUrl = Server.UrlDecode(cookie.Value);
			try
			{
				string dir = VirtualPathUtility.GetDirectory(recenSelectionUrl);
				var recentlySelected = Engine.UrlParser.Parse(dir) // was url
					?? Engine.Resolve<Navigator>().Navigate(Url.ToRelative(dir).TrimStart('~')); // was file url
				if (recentlySelected != null)
				{
					selectionTrail = new List<ContentItem>(Find.EnumerateParents(recentlySelected, null, true));
					selected = recentlySelected;
				}
			}
			catch (HttpException)
			{
			}
		}

		private bool IsAvailable(string uploadFolder)
		{
			if (string.IsNullOrEmpty(uploadFolder))
				return false;
			foreach (string availableFolder in Engine.EditManager.UploadFolders)
			{
				if (availableFolder.StartsWith(uploadFolder, StringComparison.InvariantCultureIgnoreCase))
					return true;
			}
			return false;
		}

		private void AddSiteFilesNodes(HierarchyNode<ContentItem> parent, Site site, List<ContentItem> selectionTrail)
		{
			var siteNode = Engine.Persister.Get(site.StartPageID);

			HierarchyNode<ContentItem> node = null;
			foreach (DirectoryData dd in Engine.Resolve<IContentAdapterProvider>()
				.ResolveAdapter<NodeAdapter>(siteNode)
				.GetUploadDirectories(site))
			{
				if(node == null)
					node = new HierarchyNode<ContentItem>(siteNode);
				var dir = new Directory(dd, parent.Current);
				dir.Set(FS);
				var directoryNode = CreateDirectoryNode(FS, dir, node, selectionTrail);
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

		protected override string GetToolbarSelectScript(string toolbarPluginName)
		{
			return base.GetToolbarSelectScript(toolbarPluginName);
		}
	}
}
