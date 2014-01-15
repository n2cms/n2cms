#if DEMO
using System;
using System.Configuration;
using System.IO;
using System.Web;
using N2;
using N2.Details;
using N2.Plugin;
using N2.Persistence.Serialization;
using N2.Templates.Mvc.Models.Pages;
using N2.Persistence;
using N2.Edit.Versioning;
using System.Linq;

namespace N2.Templates.Mvc.Areas.Tests.Demo
{
    [AutoInitialize]
    public class DemoInitializer : IPluginInitializer
    {
        public void Initialize(N2.Engine.IEngine factory)
        {
            if (ConfigurationManager.AppSettings["Demo.EnableContentReset"] == "true")
            {
                ReplaceContent(factory);

                if (ConfigurationManager.AppSettings["Demo.EnableUploadReset"] == "true")
                {
                    CopyFiles(factory);
                }

                factory.Persister.ItemSaving += Persister_ItemSaving;
            }
        }


        void Persister_ItemSaving(object sender, CancellableItemEventArgs e)
        {
            foreach (var cd in e.AffectedItem.Details)
            {
                if(cd.StringValue != null)
                {
                    if(cd.StringValue.Contains("script"))
                    {
                        throw new Exception("The demo site does not allow scripts to be entered.");
                    }
                }
            }
        }

        private static void CopyFiles(N2.Engine.IEngine factory)
        {
            HttpServerUtility server = HttpContext.Current.Server;
            foreach(string folder in factory.EditManager.UploadFolders)
            {
                string upload = server.MapPath(folder);
                DeleteFilesAndFolders(upload);
            }
            foreach(string file in Directory.GetFiles(server.MapPath("~/Areas/Tests/Demo/")))
            {
                if(file.EndsWith(".jpg") || file.EndsWith(".png"))
                {
                    File.Copy(file, server.MapPath("~/upload/" + Path.GetFileName(file)), true);
                }
            }
        }

        private static void DeleteFilesAndFolders(string upload)
        {
            foreach (string path in Directory.GetFiles(upload))
                File.Delete(path);
            foreach (string path in Directory.GetDirectories(upload))
                if((new System.IO.DirectoryInfo(path).Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
                    Directory.Delete(path, true);
        }


        private void ReplaceContent(N2.Engine.IEngine factory)
        {
            var installer = factory.Resolve<N2.Edit.Installation.InstallationManager>();
            installer.Install();
            var root = installer.InsertExportFile(File.OpenRead(HttpContext.Current.Server.MapPath("~/App_Data/Demo.n2.xml.gz")), "Concrete_SampleData.gz");
            if (root.ID != factory.Host.DefaultSite.RootItemID)
                factory.Host.DefaultSite.RootItemID = root.ID;
            foreach (ContentItem item in root.Children)
            {
                if (item.ID == factory.Host.DefaultSite.StartPageID && item is StartPage)
                {
                    CreateDemoPanel(factory, item);
                    return;
                }
            }

            foreach (ContentItem item in root.Children)
            {
                if (item is StartPage)
                {
                    CreateDemoPanel(factory, item);
                    factory.Host.DefaultSite.StartPageID = item.ID;
                    return;
                }
            }
        }


        private static void CreateDemoPanel(N2.Engine.IEngine factory, ContentItem item)
        {
            if (ConfigurationManager.AppSettings["Demo.CreateDemoPanel"] == "true")
            {
                var part = factory.Resolve<ContentActivator>().CreateInstance<Models.DemoPart>(item);
                part.ZoneName = "Right";
                part.SortOrder = -1000000;
                item.Children.Insert(0, part);

                factory.Persister.Save(part);
            }
        }

        private static void RemoveExistingPages(N2.Engine.IEngine factory, ContentItem rootPage)
        {
            while (rootPage.Children.Count > 0)
                factory.Persister.Delete(rootPage.Children[0]);
        }

        private static void UpdateRootPage(N2.Engine.IEngine factory, ContentItem imported, ContentItem startPage)
        {
            startPage.Title = imported.Title;
            startPage.Name = imported.Name;
            foreach (N2.Details.ContentDetail detail in imported.Details)
                startPage[detail.Name] = detail.Value;
            factory.Persister.Save(startPage);
        }

        private static void ClearPreviousVersions(N2.Engine.IEngine engine, ContentItem rootPage)
        {
            var repo = engine.Resolve<ContentVersionRepository>();
            foreach (var version in repo.Repository.Find().ToList())
                repo.Repository.Delete(version);
            //foreach (ContentItem version in factory.Resolve<N2.Persistence.Finder.IItemFinder>().Where.VersionOf.Eq(rootPage).Select())
            //    factory.Persister.Delete(version);
        }
    }
}
#endif
