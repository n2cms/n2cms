using N2.Collections;
using N2.Configuration;
using N2.Edit.FileSystem;
using N2.Engine;
using N2.Management.Api;
using N2.Web;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Principal;
using System.Web;

namespace N2.Edit.Api
{

    [Service(typeof(IApiHandler))]
    public class MediaBrowserHandler : IHttpHandler, IApiHandler
    {
        const string RegionPrivateActions = "PrivateActions";
        const string RegionPublicActions = "PublicActions";

        public MediaBrowserHandler()
            : this(Context.Current)
        {
        }

        public MediaBrowserHandler(IEngine engine)
        {
            this.engine = engine;
            FS = engine.Resolve<IFileSystem>();
        }

        protected IFileSystem FS;
        private IEngine engine;
        private SelectionUtility Selection { get { return engine.RequestContext.HttpContext.GetSelectionUtility(engine); } }

        public void ProcessRequest(HttpContext context)
        {
            ProcessRequest(context.GetHttpContextBase());
        }

        public void ProcessRequest(HttpContextBase context)
        {
            Authorize(context.User, Selection.SelectedItem);

            context.Response.SetNoCache();

            switch (context.Request.HttpMethod)
            {
                case "GET":
                    switch (context.Request.PathInfo)
                    {
                        case "":
                            WriteRetrieve(context);
                            return;
                        case "/search":
                            //WriteSearch(context);
                            return;
                    }
                    break;
                case "POST":
                    //EnsureValidSelection();
                    switch (context.Request.PathInfo)
                    {
                        case "/update":
                            //Update(context);
                            return;
                    }
                    break;
                case "DELETE":
                    switch (context.Request.PathInfo)
                    {
                        case "/message":
                            //DeleteMessage(context);
                            return;
                    }
                    break;
                case "PUT":
                    //EnsureValidSelection();
                    //Update(context);
                    return;
            }

            if (!TryExecuteExternalHandlers(context))
                throw new HttpException((int)HttpStatusCode.NotImplemented, "Not Implemented");
        }

        private bool TryExecuteExternalHandlers(HttpContextBase context)
        {
            foreach (var handler in engine.Container.ResolveAll<ContentHandlerBase>())
            {
                if (handler.Handle(context))
                    return true;
            }
            return false;
        }

        private void Authorize(IPrincipal user, ContentItem item)
        {
            if (!engine.SecurityManager.IsAuthorized(user, item, Security.Permission.Read))
                throw new UnauthorizedAccessException();
        }

        private void EnsureValidSelection()
        {
            if (Selection.ParseSelectionFromRequest() == null)
                throw new HttpException(404, "Not Found");
        }

        public bool IsReusable
        {
            get { return false; }
        }

        #region RegionPrivateActions

        private void WriteRetrieve(HttpContextBase context)
        {
            var selected = Selection.SelectedItem;
            var selectionTrail = new List<ContentItem>();
            var host = engine.Resolve<IHost>();
            var root = new HierarchyNode<ContentItem>(engine.Persister.Get(host.DefaultSite.RootItemID));

            TrySelectingPrevious(ref selected, ref selectionTrail);

            var dir = N2.Management.Files.FolderNodeProvider.CreateDirectory(root, FS, engine.Persister.Repository, engine.Resolve<IDependencyInjector>());

            if (!Engine.SecurityManager.IsAuthorized(dir, User)) { return; }

            var node = MediaBrowserHandler.CreateDirectoryNode(FS, context.Request["dir"], root, selectionTrail);


            var q = N2.Persistence.Search.Query.Parse(context.Request);
            var result = engine.Content.Search.Text.Search(q);

            context.Response.WriteJson(new
            {
                Total = result.Total,
                Hits = result
                    .Where(i => engine.SecurityManager.IsAuthorized(i, context.User))
                    .Select(i => engine.GetContentAdapter<NodeAdapter>(i).GetTreeNode(i))
                    .ToList()
            });
        }

        #endregion

        #region RegionPublicActions
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

        public void TrySelectingPrevious(ref ContentItem selected, ref List<ContentItem> selectionTrail)
        {
            var cookie = engine.RequestContext.HttpContext.Request.Cookies["lastMediaSelection"];
            if (cookie == null || string.IsNullOrEmpty(cookie.Value))
                return;

            string recenSelectionUrl = engine.RequestContext.HttpContext.Server.UrlDecode(cookie.Value);
            try
            {
                string dir = VirtualPathUtility.GetDirectory(recenSelectionUrl);
                var recentlySelected = engine.UrlParser.Parse(dir) // was url
                    ?? engine.Resolve<Navigator>().Navigate(Url.ToRelative(dir).TrimStart('~')); // was file url
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

        public bool IsAvailable(string uploadFolder)
        {
            if (string.IsNullOrEmpty(uploadFolder))
                return false;
            uploadFolder = Url.ToRelative(uploadFolder);
            foreach (var availableFolder in engine.Resolve<UploadFolderSource>().GetUploadFoldersForAllSites())
            {
                if (uploadFolder.StartsWith(Url.ToRelative(availableFolder.Path), StringComparison.InvariantCultureIgnoreCase))
                    return true;
            }

            return false;
        }

        internal static Directory CreateDirectory(FileSystemRoot folder, IFileSystem fs, IRepository<ContentItem> persister, IDependencyInjector dependencyInjector)
        {
            var dd = fs.GetDirectoryOrVirtual(folder.Path);
            var parent = persister.Get(folder.GetParentID());

            var dir = Directory.New(dd, parent, dependencyInjector);
            dir.Name = folder.GetName();
            dir.Title = folder.Title ?? dir.Name;
            dir.UrlPrefix = folder.UrlPrefix;

            Apply(folder.Readers, dir);
            Apply(folder.Writers, dir);

            return dir;
        }
        #endregion

    }

}