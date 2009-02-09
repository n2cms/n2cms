using System;
using System.Configuration;
using System.IO;
using System.Web;
using N2;
using N2.Plugin;
using N2.Serialization;
using N2.Templates.Items;

namespace Demo
{
	[AutoInitialize]
	public class DemoInitializer : IPluginInitializer
	{
		public void Initialize(N2.Engine.IEngine factory)
		{
			if (ConfigurationManager.AppSettings["ResetData"] == "true")
			{
				ReplaceContent(factory);

				CopyFiles(factory);

				N2.Edit.FileManagement.FileManager.FileUploading += FileManager_FileUploading;
				N2.Context.Current.Persister.ItemSaving += Persister_ItemSaving;
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
            File.Copy(server.MapPath("~/Addons/Demo/Resources/Sunset.jpg"), server.MapPath("~/upload/Sunset.jpg"), true);
			File.Copy(server.MapPath("~/Addons/Demo/Resources/n2.gif"), server.MapPath("~/upload/n2.gif"), true);
			File.Copy(server.MapPath("~/Addons/Demo/Resources/lav.jpg"), server.MapPath("~/upload/lav.jpg"), true);
			File.Copy(server.MapPath("~/Addons/Demo/Resources/skal.jpg"), server.MapPath("~/upload/skal.jpg"), true);
			File.Copy(server.MapPath("~/Addons/Demo/Resources/thorn.jpg"), server.MapPath("~/upload/thorn.jpg"), true);
			File.Copy(server.MapPath("~/Addons/Demo/Resources/logo_white.png"), server.MapPath("~/upload/logo_white.png"), true);
		}

		void Persister_ItemSaving(object sender, CancellableItemEventArgs e)
		{
			var file = e.AffectedItem as N2.Edit.FileSystem.Items.File;
			if(file != null)
			{
				if (!System.Text.RegularExpressions.Regex.IsMatch(file.Name, "\\.gif$|\\.jpg$|\\.jpeg$|\\.png$|\\.txt$", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
				{
					file.Delete();
					throw new Exception("Invalid extension for demo site: " + file.Name);
				}
			}
		}

		public static void FileManager_FileUploading(object sender, N2.Edit.FileManagement.FileEventArgs e)
		{
			if (!System.Text.RegularExpressions.Regex.IsMatch(e.FileName, "\\.gif$|\\.jpg$|\\.jpeg$|\\.png$|\\.txt$", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
				throw new Exception("Invalid extension for demo site: " + e.FileName);
		}

		private static void DeleteFilesAndFolders(string upload)
		{
			foreach (string path in Directory.GetFiles(upload))
				File.Delete(path);
			foreach (string path in Directory.GetDirectories(upload))
				Directory.Delete(path, true);
		}

		private void ReplaceContent(N2.Engine.IEngine factory)
		{
			Importer imp = factory.Resolve<Importer>();
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

            RootPage rootPage = factory.Persister.Get<RootPage>(factory.Host.CurrentSite.RootItemID);

			factory.SecurityManager.ScopeEnabled = false;
			((N2.Integrity.IntegrityEnforcer)factory.Resolve<N2.Integrity.IIntegrityEnforcer>()).Enabled = false;
			RemoveExistingPages(factory, rootPage);
			((N2.Integrity.IntegrityEnforcer)factory.Resolve<N2.Integrity.IIntegrityEnforcer>()).Enabled = true;
			UpdateRootPage(factory, imported, rootPage);

			imp.Import(record, rootPage, ImportOption.Children);

			foreach(ContentItem item in rootPage.Children)
				if (item is StartPage)
					factory.Resolve<N2.Web.IHost>().DefaultSite.StartPageID = item.ID;

			factory.Persister.Save(rootPage);

			//foreach (NewsList nl in Find.Items.Where.Type.Eq(typeof(NewsList)).Select())
			//{
			//    foreach (NewsContainer nc in Find.Items.Where.Type.Eq(typeof (NewsContainer)).Select())
			//    {
			//        nl.Container = nc;
			//        News n = factory.Definitions.CreateInstance<News>(nc);
			//        n.Title = "Demo site created";
			//        n.Introduction = "Today at " + DateTime.Now + " a demo site was generated for your convenience.";
			//        n.Text = "<p>Download the template web if you like.</p>";
			//        n["Syndicate"] = true;
			//        factory.Persister.Save(n);
			//    }
			//}

			//foreach (CalendarTeaser ct in Find.Items.Where.Type.Eq(typeof(CalendarTeaser)).Select())
			//{
			//    foreach (Calendar c in Find.Items.Where.Type.Eq(typeof(Calendar)).Select())
			//    {
			//        ct.Container = c;
			//        Event e = factory.Definitions.CreateInstance<Event>(c);
			//        e.Title = "Demo site scheduled for deletion";
			//        e.Introduction = "30 minutes from now the demo site will be re-created.";
			//        e.EventDate = DateTime.Now.AddMinutes(30);
			//        e["Syndicate"] = true;
			//        factory.Persister.Save(e);
			//    }
			//}

			ClearPreviousVersions(factory, rootPage);

			factory.SecurityManager.ScopeEnabled = true;
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
			foreach (N2.Details.ContentDetail detail in imported.Details.Values)
				startPage[detail.Name] = detail.Value;
			factory.Persister.Save(startPage);
		}

		private static void ClearPreviousVersions(N2.Engine.IEngine factory, ContentItem rootPage)
		{
			foreach (ContentItem version in factory.Resolve<N2.Persistence.Finder.IItemFinder>().Where.VersionOf.Eq(rootPage).Select())
				factory.Persister.Delete(version);
		}
	}
}
