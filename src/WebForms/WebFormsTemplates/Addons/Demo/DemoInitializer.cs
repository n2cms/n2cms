using System;
using System.Configuration;
using System.IO;
using System.Web;
using N2;
using N2.Plugin;
using N2.Persistence.Serialization;
using N2.Edit.Versioning;
using System.Linq;
using N2.Templates.Items;

namespace Demo
{
    [AutoInitialize]
    public class DemoInitializer : IPluginInitializer
    {
        public void Initialize(N2.Engine.IEngine engine)
        {
            if (ConfigurationManager.AppSettings["ResetData"] == "true")
            {
                ReplaceContent(engine);

                CopyFiles(engine);

                engine.Persister.ItemSaving += Persister_ItemSaving;
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

        private static void CopyFiles(N2.Engine.IEngine engine)
        {
            HttpServerUtility server = HttpContext.Current.Server;
            foreach (string folder in engine.EditManager.UploadFolders)
            {
                string upload = server.MapPath(folder);
                DeleteFilesAndFolders(upload);
            }
            File.Copy(server.MapPath("~/Addons/Demo/Resources/Sunset.jpg"), server.MapPath("~/upload/Sunset.jpg"), true);
            File.Copy(server.MapPath("~/Addons/Demo/Resources/n2.gif"), server.MapPath("~/upload/n2.gif"), true);
            File.Copy(server.MapPath("~/Addons/Demo/Resources/lav.jpg"), server.MapPath("~/upload/lav.jpg"), true);
            File.Copy(server.MapPath("~/Addons/Demo/Resources/skal.jpg"), server.MapPath("~/upload/skal.jpg"), true);
            File.Copy(server.MapPath("~/Addons/Demo/Resources/thorn.jpg"), server.MapPath("~/upload/thorn.jpg"), true);
            File.Copy(server.MapPath("~/Addons/Demo/Resources/logo_white.png"), server.MapPath("~/upload/logo_white.png"), true);
        }

        private static void DeleteFilesAndFolders(string upload)
        {
            foreach (string path in Directory.GetFiles(upload))
                File.Delete(path);
            foreach (string path in Directory.GetDirectories(upload))
                Directory.Delete(path, true);
        }

        private void ReplaceContent(N2.Engine.IEngine engine)
        {
            Importer imp = engine.Resolve<Importer>();
            IImportRecord record;
            HttpContext context = HttpContext.Current;
            if (context != null && File.Exists(context.Server.MapPath("~/App_Data/export.n2.xml.gz")))
            {
                record = imp.Read(context.Server.MapPath("~/App_Data/export.n2.xml.gz"));
            }
            else
            {
                record = imp.Read(GetType().Assembly.GetManifestResourceStream("Demo.Resources.export.n2.xml.gz"), "export.n2.xml.gz");
            }

            ContentItem imported = record.RootItem;

            ContentItem rootPage = engine.Persister.Get<ContentItem>(engine.Host.DefaultSite.RootItemID);

            engine.SecurityManager.ScopeEnabled = false;
            try
            {
                ((N2.Integrity.IntegrityEnforcer)engine.Resolve<N2.Integrity.IIntegrityEnforcer>()).Enabled = false;
                RemoveExistingPages(engine, rootPage);
            }
            finally
            {
                ((N2.Integrity.IntegrityEnforcer)engine.Resolve<N2.Integrity.IIntegrityEnforcer>()).Enabled = true;
            }
            UpdateRootPage(engine, imported, rootPage);

            imp.Import(record, rootPage, ImportOption.Children);

            foreach(ContentItem item in rootPage.Children)
                if (item is StartPage)
                    engine.Host.DefaultSite.StartPageID = item.ID;

            engine.Persister.Save(rootPage);

            //foreach (NewsList nl in Find.Items.Where.Type.Eq(typeof(NewsList)).Select())
            //{
            //    foreach (NewsContainer nc in Find.Items.Where.Type.Eq(typeof (NewsContainer)).Select())
            //    {
            //        nl.Container = nc;
            //        News n = engine.Definitions.CreateInstance<News>(nc);
            //        n.Title = "Demo site created";
            //        n.Introduction = "Today at " + DateTime.Now + " a demo site was generated for your convenience.";
            //        n.Text = "<p>Download the template web if you like.</p>";
            //        n["Syndicate"] = true;
            //        engine.Persister.Save(n);
            //    }
            //}

            //foreach (CalendarTeaser ct in Find.Items.Where.Type.Eq(typeof(CalendarTeaser)).Select())
            //{
            //    foreach (Calendar c in Find.Items.Where.Type.Eq(typeof(Calendar)).Select())
            //    {
            //        ct.Container = c;
            //        Event e = engine.Definitions.CreateInstance<Event>(c);
            //        e.Title = "Demo site scheduled for deletion";
            //        e.Introduction = "30 minutes from now the demo site will be re-created.";
            //        e.EventDate = DateTime.Now.AddMinutes(30);
            //        e["Syndicate"] = true;
            //        engine.Persister.Save(e);
            //    }
            //}

            ClearPreviousVersions(engine, rootPage);

            engine.SecurityManager.ScopeEnabled = true;
        }

        private static void RemoveExistingPages(N2.Engine.IEngine engine, ContentItem rootPage)
        {
            while (rootPage.Children.Count > 0)
                engine.Persister.Delete(rootPage.Children[0]);
        }

        private static void UpdateRootPage(N2.Engine.IEngine engine, ContentItem imported, ContentItem startPage)
        {
            startPage.Title = imported.Title;
            startPage.Name = imported.Name;
            foreach (N2.Details.ContentDetail detail in imported.Details.Values)
                startPage[detail.Name] = detail.Value;
            engine.Persister.Save(startPage);
        }

        private static void ClearPreviousVersions(N2.Engine.IEngine engine, ContentItem rootPage)
        {
            var repo = engine.Resolve<ContentVersionRepository>();
            foreach(var version in repo.Repository.Find().ToList())
                repo.Repository.Delete(version);
            //foreach (ContentItem version in engine.Resolve<N2.Persistence.Finder.IItemFinder>().Where.VersionOf.Eq(rootPage).Select())
            //    engine.Persister.Delete(version);
        }
    }
}
