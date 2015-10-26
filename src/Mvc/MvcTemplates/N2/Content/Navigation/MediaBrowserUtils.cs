using N2.Collections;
using N2.Edit.FileSystem;
using N2.Engine;
using System;
using System.Collections.Generic;
using System.Web;

namespace N2.Edit.Navigation
{
    public class MediaBrowserUtils
    {

        public static List<HierarchyNode<ContentItem>> GetAvailableUploadFoldersForAllSites(
            HttpContext context,
            HierarchyNode<ContentItem> root,
            List<ContentItem> selectionTrail,
            IEngine engine, IFileSystem FS)
        {
            var uploadDirectories = new List<HierarchyNode<ContentItem>>();
            foreach (var upload in engine.Resolve<UploadFolderSource>().GetUploadFoldersForAllSites())
            {
                var dir = N2.Management.Files.FolderNodeProvider.CreateDirectory(upload, FS, engine.Persister.Repository, engine.Resolve<IDependencyInjector>());

                if (!engine.SecurityManager.IsAuthorized(dir, context.User))
                    continue;

                var node = CreateDirectoryNode(FS, dir, root, selectionTrail);
                uploadDirectories.Add(node);
            }
            return uploadDirectories;
        }

        public static HierarchyNode<ContentItem> CreateDirectoryNode(IFileSystem fs, ContentItem directory, HierarchyNode<ContentItem> parent, List<ContentItem> selectionTrail)
        {
            var node = new HierarchyNode<ContentItem>(directory);
            ExpandRecursive(fs, node, selectionTrail);

            return node;
        }

        public static void ExpandRecursive(IFileSystem fs, HierarchyNode<ContentItem> parent, List<ContentItem> selectionTrail)
        {
            int index = selectionTrail.FindIndex(ci => string.Equals(ci.Url, parent.Current.Url, StringComparison.InvariantCultureIgnoreCase));
            if (index < 0) return;
            foreach (var child in parent.Current.GetChildPagesUnfiltered())
            {
                parent.Children.Add(CreateDirectoryNode(fs, child, parent, selectionTrail));
            }
        }


    }

    public class MediaBrowserModel
    {
        public IList<FileReducedListModel> Files { get; set; }
        public IList<FileSystem.Items.Directory> Dirs { get; set; }
        public string Path { get; set; }
        public string CkEditor { get; set; }
        public string CkEditorFuncNum { get; set; }
        public string MediaControl { get; set; }
        public string PreferredSize { get; set; }
        public string HandlerUrl { get; set; }
        public string[] Breadcrumb { get; set; }
        public bool RootIsSelectable { get; set; }
    }

    public class FileReducedListModel
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public bool IsImage { get; set; }
        public string Thumb { get; set; }
        public long Size { get; set; }
        public string Date { get; set; }
        public int SCount { get; set; }
        public List<FileReducedChildrenModel> Children { get; set; }
    }

    public class FileReducedChildrenModel
    {
        public string SizeName { get; set; }
        public string Url { get; set; }
        public long Size { get; set; }
    }

    public class UploadFileInfo
    {
        public string Url { get; set; }
        public string AltText { get; set; }
        public string Ext { get; set; }
        public DateTime Date { get; set; }
        public long Size { get; set; }
        public string ImageSize { get; set; }
        public bool IsImage { get; set; }
        public int Id { get; set; }
        public string[] Relations { get; set; }
    }
    public class UploadFileResponse
    {
        public string Status { get; set; }
        public string ControlId { get; set; }
        public IEnumerable<UploadFileResponseUrl> Urls { get; set; }
    }

    public class UploadFileResponseUrl
    {
        public string Url { get; set; }
        public int Id { get; set; }
        public long Size { get; set; }
    }

    public class UploadCheckListModel
    {
        public string[] Filenames { get; set; }
    }
}