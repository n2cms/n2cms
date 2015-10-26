using N2.Collections;
using N2.Edit.FileSystem;
using N2.Edit.FileSystem.Items;
using N2.Engine;
using N2.Web;
using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Web;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using System.Collections.Specialized;
using System.Web.Security;
using System.Collections;

namespace N2.Edit.Navigation
{
    /// <summary>
    /// Descripción breve de MediaBrowserHandler
    /// </summary>
    public class MediaBrowserHandler : IHttpHandler
    {
        const string RegionPrivateActions = "PrivateActions";
        const string RegionHelpers = "HelperUtils";

        protected IFileSystem FS;
        protected N2.Management.Files.FileSystem.Pages.ImageSizeCache ImageSizes { get { return Engine.Resolve<Management.Files.FileSystem.Pages.ImageSizeCache>(); } }
        private SelectionUtility Selection { get { return Engine.RequestContext.HttpContext.GetSelectionUtility(Engine); } }
        protected IEngine Engine;
        protected ContentItem GetSelectedItem(NameValueCollection queryString)
        {
            string path = queryString[SelectionUtility.SelectedQueryKey];
            return N2.Context.Current.Resolve<N2.Edit.Navigator>().Navigate(path);
        }

        public virtual bool IsReusable
        {
            get { return false; }
        }

        public MediaBrowserHandler()
            : this(Context.Current)
        {
        }

        public MediaBrowserHandler(IEngine engine)
        {
            Engine = engine;
        }

        public void ProcessRequest(HttpContext context)
        {
            Authorize(context.User, Selection.SelectedItem);
            context.Response.AddHeader("Pragma", "no-cache");
            context.Response.AddHeader("Cache-Control", "private, no-cache");

            switch (context.Request.HttpMethod)
            {
                case "GET":
                    switch (context.Request.PathInfo)
                    {
                        case "":
                        case "/":
                            WriteRetrieve(context);
                            return;
                        case "/search":
                            WriteSearch(context);
                            return;
                    }
                    break;
                case "POST":
                    switch (context.Request.PathInfo)
                    {
                        case "/checkIfExists":
                            WriteCheckIfExists(context);
                            return;
                        case "/uploadFile":
                            WriteUploadFile(context);
                            return;
                    }
                    break;
            }

        }

        #region RegionHelpers

        private void Authorize(IPrincipal user, ContentItem item)
        {
            if (!Engine.SecurityManager.IsAuthorized(user, item, N2.Security.Permission.Read))
                throw new UnauthorizedAccessException();
        }

        public void TrySelectingPrevious(ref ContentItem selected, ref List<ContentItem> selectionTrail)
        {
            var cookie = Engine.RequestContext.HttpContext.Request.Cookies["lastMediaSelection"];
            if (cookie == null || string.IsNullOrEmpty(cookie.Value))
                return;

            string recenSelectionUrl = Engine.RequestContext.HttpContext.Server.UrlDecode(cookie.Value);
            try
            {
                string dir = VirtualPathUtility.GetDirectory(recenSelectionUrl);
                var recentlySelected = Engine.Resolve<Navigator>().Navigate(Url.ToRelative(dir).TrimStart('~')); 
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

        private string AbsolutePathToVirtual(string absPath)
        {
            var hs = System.Web.Hosting.HostingEnvironment.MapPath("~");
            return "/" + absPath.Replace(hs, "").Replace('\\','/');
        }

        class TitleComparer<T> : IComparer<T> where T : ContentItem
        {
            public int Compare(T x, T y)
            {
                return StringComparer.InvariantCultureIgnoreCase.Compare(x.Title, y.Title);
            }
        }

        public static List<FileReducedListModel> GetFileReducedList(List<File> files, Management.Files.FileSystem.Pages.ImageSizeCache imageSizes, string exts = "")
        {
            var regIsImage = new Regex(@"^.*\.(jpg|jpeg|gif|png)$", RegexOptions.IgnoreCase);

            var ret = files.Select(d => new FileReducedListModel
            {
                Title = d.Title,
                Url = d.LocalUrl,
                IsImage = regIsImage.IsMatch(d.Title),
                Thumb = regIsImage.IsMatch(d.Title) ? N2.Web.Drawing.ImagesUtility.GetExistingImagePath(d.LocalUrl, "thumb") : null,
                Size = d.Size,
                Date = string.Format("{0:s}", d.Created),
                SCount = d.Children.Count,
                Children = regIsImage.IsMatch(d.Title) ? GetFileReducedChildren(d, imageSizes) : null
            }).ToList();


            if (!string.IsNullOrEmpty(exts))
            {
                var eqComparer = new ExtensionEqualityComparer();
                var extsArray = exts.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                var ret2 = ret.Where(p => extsArray.Contains(p.Url, eqComparer)).ToList();
                return ret2;
            }

            return ret;
        }

        private static List<FileReducedChildrenModel> GetFileReducedChildren(File file, Management.Files.FileSystem.Pages.ImageSizeCache imageSizes)
        {
            if (file.Children == null) return null;
            try
            {
                return file.Children.Select(
                    cc => new FileReducedChildrenModel
                    {
                        SizeName = imageSizes.GetSizeName(cc.Title),
                        Url = cc.Url,
                        Size = (cc as File) != null ? (cc as File).Size : -1
                    }
                    ).ToList();
            }
            catch
            {
                return null;
            }
        }

        private static void ValidateTicket(string encryptedTicket)
        {
            var ticket = FormsAuthentication.Decrypt(encryptedTicket);
            if (ticket.Expired)
                throw new N2.Security.PermissionDeniedException("Upload ticket expired");
            if (!ticket.Name.StartsWith("SecureUpload-"))
                throw new N2.Security.PermissionDeniedException("Unknown ticket");
        }

        private class ExtensionEqualityComparer : IEqualityComparer<string>
        {
            public bool Equals(string a, string b)
            {
                return string.Compare(a, System.IO.Path.GetExtension(b), true) == 0;
            }

            public int GetHashCode(string bx)
            {
                return bx.GetHashCode();
            }
        }

        #endregion

        #region RegionPrivateActions 

        private void WriteRetrieve(HttpContext context)
        {
            IList<Directory> dirs;
            IList<File> files;

            var host = Engine.Resolve<IHost>();
            var rootItem = Engine.Persister.Get(host.DefaultSite.RootItemID);
            var root = new HierarchyNode<ContentItem>(rootItem);

            var selected = Selection.SelectedItem;
            var selectionTrail = Find.EnumerateParents(selected, null, true).ToList().Where(a => a is AbstractNode).Reverse().ToList();
            var selectedPath = selected.Path;

            var dir = selected as Directory;
            if (dir == null)
            {
                FS = Engine.Resolve<IFileSystem>();
                var uploadDirectories = MediaBrowserUtils.GetAvailableUploadFoldersForAllSites(context, root, selectionTrail, Engine, FS);
                dirs = new List<Directory>();
                files = new List<File>();
                foreach (var updDir in uploadDirectories)
                {
                    dirs.Add(updDir.Current as Directory);
                }
                selectedPath = "/";
            } else
            {
                dirs = dir.GetDirectories();
                files = dir.GetFiles();
            }

            var selectableExtensions = context.Request["exts"];

            context.Response.WriteJson(new
            {
                Path = selectedPath,
                Total = dirs.Count + files.Count,
                Trail = selectionTrail.Select(d => new { d.Title, Url = d.Url }).ToList(),
                Dirs = dirs.Select(d => new { d.Title, Url = d.LocalUrl }).ToList(),
                Files = GetFileReducedList(files.ToList(), ImageSizes, selectableExtensions)
            });
        }

        private void WriteSearch(HttpContext context)
        {
            var query = context.Request["query"];
            if (string.IsNullOrWhiteSpace(query))
            {
                context.Response.WriteJson(new { Total = 0, Message = "Please provide a search term" });
                return;
            }
            query = query.TrimStart().TrimEnd();

            var host = Engine.Resolve<IHost>();
            var rootItem = Engine.Persister.Get(host.DefaultSite.RootItemID);
            var root = new HierarchyNode<ContentItem>(rootItem);

            FS = Engine.Resolve<IFileSystem>();
            var selectionTrail = Find.EnumerateParents(Selection.SelectedItem, null, true).ToList().Where(a => a is AbstractNode).Reverse().ToList();
            var uploadDirectories = MediaBrowserUtils.GetAvailableUploadFoldersForAllSites(context, root, selectionTrail, Engine, FS);

            if (uploadDirectories.Count == 0)
            {
                context.Response.WriteJson(new { Total = 0, Message = "No available directories" });
                return;
            }

            if (query.IndexOf('*') < 0)
            {
                query = "*" + query + "*";
            }

            var resultFilenames = new List<string>();
            foreach(var dir in uploadDirectories)
            {
                //Search, returns Absolute Paths
                resultFilenames.AddRange(
                    System.IO.Directory.GetFiles(
                        System.Web.Hosting.HostingEnvironment.MapPath("~" + dir.Current.Url), 
                        query, 
                        System.IO.SearchOption.AllDirectories
                        )
                    );
            }

            if (resultFilenames.Count == 0)
            {
                context.Response.WriteJson(new { Total = 0, Message = "0 files found" });
                return;
            }

            List<File> files = new List<File>();
            var fileMap = new Dictionary<string, File>(StringComparer.OrdinalIgnoreCase);
            string lastParent = null;
            AbstractDirectory parent = null;

            foreach (var nm in resultFilenames.OrderBy(s => s))
            {
                var fd = FS.GetFile(AbsolutePathToVirtual(nm));
                var parentDirectory = fd.VirtualPath.Substring(0, fd.VirtualPath.LastIndexOf('/') );
                if(lastParent != parentDirectory)
                {
                    parent = new Directory(DirectoryData.Virtual(parentDirectory), null);
                    lastParent = parentDirectory;
                }

                var file = new File(fd, parent);
                file.Set(FS);
                file.Set(ImageSizes);

                var unresizedFileName = ImageSizes.RemoveImageSize(file.Name);
                if (unresizedFileName != null && fileMap.ContainsKey(unresizedFileName))
                {
                    fileMap[unresizedFileName].Add(file);

                    if (ImageSizes.GetSizeName(file.Name) == "icon")
                        file.IsIcon = true;
                }
                else
                {
                    if (unresizedFileName == null)
                    {
                        files.Add(file);
                        fileMap[file.Name] = file;
                    }
                }
            }
            files.Sort(new TitleComparer<File>());

            var selectableExtensions = context.Request["exts"];

            context.Response.WriteJson(new
            {
                Path = "",
                Total = files.Count,
                Files = GetFileReducedList(files, ImageSizes, selectableExtensions)
            });

        }

        private void WriteCheckIfExists(HttpContext context)
        {
            var parentDirectory = context.Request["selected"];
            var filenames = Selection.RequestValueAccessor("filenames");
            var selected = new Directory(DirectoryData.Virtual(parentDirectory), null);

            if (string.IsNullOrEmpty(parentDirectory) || !Engine.SecurityManager.IsAuthorized(context.User, selected, N2.Security.Permission.Read))
            {
                context.Response.WriteJson(new { Status = "Error", Message = "Not allowed" });
                return;
            }

            if (string.IsNullOrEmpty(filenames))
            {
                context.Response.WriteJson(new { Status = "Error", Message = "No files selected" });
                return;
            }

            var fns = filenames.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

            context.Response.WriteJson(
                new
                {
                    Status = "Checked",
                    Files = (from fileName in fns
                             let newPath = System.IO.Path.Combine(context.Server.MapPath(selected.Url), fileName)
                             where System.IO.File.Exists(newPath)
                             select fileName).ToArray()
                });
        }

        private void WriteUploadFile(HttpContext context)
        {
            ValidateTicket(context.Request["ticket"]);
            FS = Engine.Resolve<IFileSystem>();

            var parentDirectory = context.Request["selected"];
            var selected = new Directory(DirectoryData.Virtual(parentDirectory), null);

            if (string.IsNullOrEmpty(parentDirectory) || !Engine.SecurityManager.IsAuthorized(context.User, selected, N2.Security.Permission.Write))
            {
                context.Response.WriteJson(new { Status = "Error", Message = "Not allowed" });
                return;
            }

            var overwriteStr = Selection.RequestValueAccessor("overwrite");
            var overwrite = overwriteStr.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

            try
            {
                if (context.Request.Files.Count == 0)
                {
                    context.Response.WriteJson(new { Status = "Ok", Message = "No files uploaded" });
                    return;
                }

                foreach (string file in context.Request.Files)
                {
                    var fileContent = context.Request.Files[file];
                    if (fileContent == null || fileContent.ContentLength <= 0) continue;

                    var stream = fileContent.InputStream;
                    var fileName = System.IO.Path.GetFileName(fileContent.FileName);
                    if (string.IsNullOrEmpty(fileName)) continue;

                    var newPath = System.IO.Path.Combine(context.Server.MapPath(parentDirectory), fileName);

                    if (overwrite != null && overwrite.Any(p => p == fileName))
                    {
                        var vPath = selected.Url + fileName;
                        FS.WriteFile(vPath, stream);
                    } // Overwrite = yes
                    else
                    {
                        var incr = 1;
                        while (System.IO.File.Exists(newPath))
                        {
                            fileName = string.Format("{0}-{1}{2}", System.IO.Path.GetFileNameWithoutExtension(fileName), incr, System.IO.Path.GetExtension(fileName));
                            newPath = System.IO.Path.Combine(context.Server.MapPath(parentDirectory), fileName);
                        }

                        var vPath = selected.Url + fileName;
                        FS.WriteFile(vPath, stream);
                    } // Overwrite = no
                }
                context.Response.WriteJson(new { Status = "Ok", Message = "Files uploaded" });
                return;
            }
            catch (Exception)
            {
                context.Response.WriteJson(new { Status = "Error", Message = "Upload failed" });
                return;
            }

        }

        #endregion
    }
}