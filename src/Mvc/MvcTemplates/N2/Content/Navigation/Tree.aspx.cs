using System;
using System.Linq;
using System.Collections.Generic;
using System.Web;
using System.Web.UI.HtmlControls;
using N2.Collections;
using N2.Edit.FileSystem;
using N2.Edit.FileSystem.Items;
using N2.Engine;
using N2.Resources;
using N2.Web;
using N2.Configuration;
using N2.Edit;
using N2.Edit.Workflow;
using N2.Persistence;
using N2.Management.Files.FileSystem.Pages;

namespace N2.Edit.Navigation
{
    public partial class Tree : NavigationPage
    {
        protected HtmlInputHidden inputLocation;
        protected HtmlInputFile inputFile;
        protected IFileSystem FS;
        protected EditSection Config;
        
        protected override void OnInit(EventArgs e)
        {
            FS = Engine.Resolve<IFileSystem>();
            Config = Engine.Resolve<ConfigurationManagerWrapper>().Sections.Management;
            Register.JQueryUi(Page);
            var selected = Selection.SelectedItem;
            if (IsPostBack && !string.IsNullOrEmpty(inputFile.PostedFile.FileName))
            {
                string uploadFolder = Request[inputLocation.UniqueID];
                if(!IsAvailable(uploadFolder))
                    throw new N2Exception("Cannot upload to " + Server.HtmlEncode(uploadFolder));

                string fileName = System.IO.Path.GetFileName(inputFile.PostedFile.FileName);
                string filePath = Url.Combine(uploadFolder, fileName);

                if (Engine.Config.Sections.Management.UploadFolders.IsTrusted(fileName))
                {
                    FS.WriteFile(filePath, inputFile.PostedFile.InputStream);
                }
                else
                {
                    throw new N2.Security.PermissionDeniedException("Invalid file name");
                }

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

                foreach (var upload in Engine.Resolve<UploadFolderSource>().GetUploadFoldersForAllSites())
                {
                    var dir = N2.Management.Files.FolderNodeProvider.CreateDirectory(upload, FS, Engine.Persister.Repository, Engine.Resolve<IDependencyInjector>());

                    if (!Engine.SecurityManager.IsAuthorized(dir, User))
                        continue;

                    var node = CreateDirectoryNode(FS, dir, root, selectionTrail);
                    root.Children.Add(node);
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

            siteTreeView.SelectableTypes = Request["selectableTypes"];
            siteTreeView.SelectableExtensions = Request["selectableExtensions"];
            siteTreeView.DataBind();

            inputLocation.Value = siteTreeView.SelectedItem.Url;

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
            catch (ArgumentException)
            {
            }
            catch (HttpException)
            {
            }
        }

        private bool IsAvailable(string uploadFolder)
        {
            if (string.IsNullOrEmpty(uploadFolder))
                return false;
            uploadFolder = Url.ToRelative(uploadFolder);
            foreach (var availableFolder in Engine.Resolve<UploadFolderSource>().GetUploadFoldersForAllSites())
            {
                if (uploadFolder.StartsWith(Url.ToRelative(availableFolder.Path), StringComparison.InvariantCultureIgnoreCase))
                    return true;
            }

            return false;
        }

        private void AddSiteFilesNodes(HierarchyNode<ContentItem> parent, Site site, List<ContentItem> selectionTrail)
        {
            var startPage = Engine.Persister.Get(site.StartPageID);
            var sizes = Engine.Resolve<ImageSizeCache>();

            HierarchyNode<ContentItem> node = null;
            foreach(var dir in Engine.GetContentAdapter<NodeAdapter>(startPage)
                .GetChildren(startPage, Interfaces.Managing)
                .OfType<Directory>())
            {
                if (node == null)
                    node = new HierarchyNode<ContentItem>(startPage);
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
	        if (index < 0) return;
	        foreach (var child in parent.Current.GetChildPagesUnfiltered())
	        {
		        parent.Children.Add(CreateDirectoryNode(fs, child, parent, selectionTrail));
	        }
        }

        protected override string GetToolbarSelectScript(string toolbarPluginName)
        {
            return base.GetToolbarSelectScript(toolbarPluginName);
        }
    }
}
