using N2.Collections;
using N2.Configuration;
using N2.Edit.FileSystem;
using N2.Edit.FileSystem.Items;
using N2.Resources;
using N2.Web;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Configuration;
using System.Web.Security;
using System.Web.UI.HtmlControls;
using System.Web;

namespace N2.Edit.Navigation
{
    public partial class MediaBrowser : NavigationPage
    {
        protected HtmlInputHidden inputLocation;
        protected HtmlInputFile inputFile;
        protected IFileSystem FS;
        protected EditSection Config;

        public const int TakeDefault = 40;
        public const string AjaxMediaBrowserRetriever = "MediaBrowserHandler.ashx";
        public MediaBrowserModel mediaBrowserModel;

        protected override void OnInit(EventArgs e)
        {
            FS = Engine.Resolve<IFileSystem>();
            Config = Engine.Resolve<ConfigurationManagerWrapper>().Sections.Management;
            var selected = Selection.SelectedItem;

            var host = Engine.Resolve<IHost>();
            var rootItem = Engine.Persister.Get(host.DefaultSite.RootItemID);
            var root = new HierarchyNode<ContentItem>(rootItem);
            var selectionTrail = new List<ContentItem>();

            var ckEditor = Request["ckEditor"];
            var ckEditorFuncNum = Request[ "ckEditorFuncNum"];
            var mediaCtrl = Request["mediaControl"];
            var preferredSize = Request["preferredSize"];
            var selectableExtensions = Request["selectableExtensions"];
            var selectedUrl = Request["selectedUrl"];
            var defaultDirectoryPath = Request["defaultDirectoryPath"];
            var mediaBrowserHandler = new MediaBrowserHandler();

            mediaBrowserModel = new MediaBrowserModel
            {
                CkEditor = ckEditor,
                CkEditorFuncNum = ckEditorFuncNum,
                MediaControl = mediaCtrl,
                HandlerUrl = AjaxMediaBrowserRetriever,
                PreferredSize = preferredSize,
                Breadcrumb = new string[] { },
                Path = "",
                RootIsSelectable = false,
                DefaultDirectoryPath = defaultDirectoryPath
            };


            if (selected is AbstractNode)
            {
                selectionTrail = new List<ContentItem>(Find.EnumerateParents(selected, null, true));
            }
            else
            {
                mediaBrowserHandler.TrySelectingPrevious(ref selected, ref selectionTrail);
            }

            var uploadDirectories = MediaBrowserUtils.GetAvailableUploadFoldersForAllSites(Context, root, selectionTrail, Engine, FS);

            if (uploadDirectories.Count > 0)
            {
                var imageSizes = Engine.Resolve<Management.Files.FileSystem.Pages.ImageSizeCache>();
                if (uploadDirectories.Count == 1 || selectionTrail.Count > 1)
                {
                    var directory = FS.GetDirectory(mediaBrowserModel.Path);
                    var fsRootPath = directory != null && !string.IsNullOrWhiteSpace(directory.RootPath) ? directory.RootPath : "";

                    var dir = (selectionTrail.Count > 0 ? selectionTrail.ElementAt(0) : uploadDirectories[0].Current) as Directory;


                    //Find and/or create default upload directory for templated content.
                    if (!string.IsNullOrWhiteSpace(mediaBrowserModel.DefaultDirectoryPath))
                    {
                        var segments = (mediaBrowserModel.DefaultDirectoryPath.Split(new[] { '/' }, 3, StringSplitOptions.RemoveEmptyEntries));
                        if (segments.Length == 3)
                        {
                            var siteName = segments[0];
                            var uploadSiteFolder = dir.GetDirectories().FirstOrDefault(d => d.Name == siteName.ToLower());
                            if (uploadSiteFolder == null)
                            {
                                var newDir = VirtualPathUtility.AppendTrailingSlash(Request.ApplicationPath + dir.LocalUrl) + siteName.ToLower();
                                FS.CreateDirectory(newDir);
                                uploadSiteFolder = dir.GetDirectories().FirstOrDefault(d => d.Name == siteName.ToLower());
                            }

                            var contentName = segments[1];
                            var uploadSiteContentFolder = uploadSiteFolder.GetDirectories().FirstOrDefault(d => d.Name == contentName.ToLower());
                            if (uploadSiteContentFolder == null)
                            {
                                var newDir = VirtualPathUtility.AppendTrailingSlash(Request.ApplicationPath + uploadSiteFolder.LocalUrl) + contentName.ToLower();
                                FS.CreateDirectory(newDir);
                                uploadSiteContentFolder = uploadSiteFolder.GetDirectories().FirstOrDefault(d => d.Name == contentName.ToLower());
                            }

                            var templateName = segments[2];
                            var uploadSiteContentTemplateFolder = uploadSiteContentFolder.GetDirectories().FirstOrDefault(d => d.Name == templateName.ToLower());
                            if (uploadSiteContentTemplateFolder == null)
                            {
                                var newDir = VirtualPathUtility.AppendTrailingSlash(Request.ApplicationPath + uploadSiteContentFolder.LocalUrl) + templateName.ToLower();
                                FS.CreateDirectory(newDir);
                                uploadSiteContentTemplateFolder = uploadSiteContentFolder.GetDirectories().FirstOrDefault(d => d.Name == templateName.ToLower());

                                dir = uploadSiteContentTemplateFolder;
                            }
                            else
                            {
                                dir = uploadSiteContentTemplateFolder;
                            }

                            var monthName = DateTime.Now.ToString("yyyy-MM");
                            var uploadSiteContentTemplateMonthFolder = uploadSiteContentTemplateFolder.GetDirectories().FirstOrDefault(d => d.Name == monthName.ToLower());
                            if (uploadSiteContentTemplateMonthFolder == null)
                            {
                                var newDir = VirtualPathUtility.AppendTrailingSlash(Request.ApplicationPath + uploadSiteContentTemplateFolder.LocalUrl) + monthName.ToLower();
                                FS.CreateDirectory(newDir);
                                uploadSiteContentTemplateMonthFolder = uploadSiteContentTemplateFolder.GetDirectories().FirstOrDefault(d => d.Name == monthName.ToLower());

                                dir = uploadSiteContentTemplateMonthFolder;
                            }
                            else
                            {
                                dir = uploadSiteContentTemplateMonthFolder;
                            }
                        } 
                    }
                    

                    var files = dir.GetFiles().ToList();
                    mediaBrowserModel.Dirs = dir.GetDirectories();
                    mediaBrowserModel.Files = MediaBrowserHandler.GetFileReducedList(files, imageSizes, selectableExtensions, fsRootPath);
                    mediaBrowserModel.Path = System.Web.VirtualPathUtility.ToAppRelative(dir.LocalUrl).Trim('~');
                }
                else
                {
                    mediaBrowserModel.Path = System.Web.VirtualPathUtility.ToAppRelative("/").Trim('~');
                    mediaBrowserModel.RootIsSelectable = true;
                    mediaBrowserModel.Dirs = new List<Directory>();
                    foreach (var updDir in uploadDirectories)
                    {
                        mediaBrowserModel.Dirs.Add(updDir.Current as Directory);
                    }
                }

                var breadcrumb = mediaBrowserModel.Path.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                breadcrumb.Insert(0, "[root]");
                mediaBrowserModel.Breadcrumb = breadcrumb.ToArray();
           }

        }

		protected string GetEncryptedTicket()
		{
			return FormsAuthentication.Encrypt(new FormsAuthenticationTicket("SecureUpload-" + Guid.NewGuid(), false, 60));
        }

		protected int GetMaxSize()
		{
			try
			{
				return (ConfigurationManager.GetSection("system.web/httpRuntime") as HttpRuntimeSection) != null ? (ConfigurationManager.GetSection("system.web/httpRuntime") as HttpRuntimeSection).MaxRequestLength : -1;
			}
			catch
			{
				return -1;
			}
        }
        
    }
}