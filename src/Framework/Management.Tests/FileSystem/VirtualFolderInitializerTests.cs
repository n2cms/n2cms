using System.Linq;
using N2.Collections;
using N2.Configuration;
using N2.Edit.Installation;
using N2.Management.Files;
using N2.Tests;
using N2.Web;
using NUnit.Framework;
using N2.Edit.FileSystem.Items;
using Rhino.Mocks;
using N2.Persistence;
using N2.Engine;
using N2.Tests.Fakes;
using N2.Edit.FileSystem;
using System;
using Shouldly;
using N2.Management.Files.FileSystem.Pages;

namespace N2.Edit.Tests.FileSystem
{
    [TestFixture]
    public class VirtualFolderInitializerTests : ItemPersistenceMockingBase
    {
        Host host;
        FakeMappedFileSystem fs;
        VirtualNodeFactory vnf;
        EditSection config;
        VirtualFolderInitializer initializer;
        FakeDependencyInjector injector;

        ContentItem root;
        ContentItem start;
        FolderNodeProvider nodeProvider;
        private Plugin.ConnectionMonitor monitor;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            root = CreateOneItem<RootNode>(1, "root", null);
            start = CreateOneItem<RootNode>(2, "start", root);
            host = new Host(new ThreadContext(), root.ID, start.ID);

            fs = new FakeMappedFileSystem();
            fs.BasePath = AppDomain.CurrentDomain.BaseDirectory + @"\FileSystem\";

            vnf = new VirtualNodeFactory();
            config = new EditSection();

            injector = new FakeDependencyInjector();
            injector.injectors.Add(new EntityDependencySetter<IFileSystem>(fs));
            injector.injectors.Add(new EntityDependencySetter<IDependencyInjector>(injector));
            var sizeCache = new ImageSizeCache(new ConfigurationManagerWrapper { Sections = new ConfigurationManagerWrapper.ContentSectionTable(null, null, null, config) });
            injector.injectors.Add(new EntityDependencySetter<ImageSizeCache>(sizeCache));
            //nodeProvider = new FolderNodeProvider(fs, persister, injector);
            //initializer = new VirtualFolderInitializer(host, persister, fs, vnf, new Plugin.ConnectionMonitor().SetConnected(SystemStatusLevel.UpAndRunning), config, new ImageSizeCache(new ConfigurationManagerWrapper { Sections = new ConfigurationManagerWrapper.ContentSectionTable(null, null, null, config) }), nodeProvider);
            //nodeProvider = new FolderNodeProvider(fs, persister, injector);
            //nodeProvider = new FolderNodeProvider(fs, persister, injector);
            nodeProvider = new FolderNodeProvider(fs, persister.Repository, injector);
            initializer = new VirtualFolderInitializer(host, persister, fs, vnf, monitor = new Plugin.ConnectionMonitor().SetConnected(SystemStatusLevel.UpAndRunning), new UploadFolderSource(host, config), nodeProvider);
        }

        class FakeStatus : DatabaseStatusCache
        {
            public FakeStatus()
                : base(null)
            {
            }

            public override SystemStatusLevel GetStatus()
            {
                return SystemStatusLevel.UpAndRunning;
            }
        }


        [Test]
        public void Get_UploadFolder()
        {
            initializer.Start();
            vnf.Register(nodeProvider);

            var result = vnf.Get("/upload/");

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo("upload"));
        }

        [Test]
        public void UploadFolder_IsNotAvailable_FromStartPage()
        {
            initializer.Start();
            vnf.Register(nodeProvider);

            var result = vnf.Get("/start/upload/");

            Assert.That(result, Is.Null);
        }

        [Test]
        public void Get_ChildTo_UploadFolder()
        {
            initializer.Start();
            vnf.Register(nodeProvider);

            var result = vnf.Get("/upload/File.txt");

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo("File.txt"));
        }

        [Test]
        public void GetChildren_Includes_UploadFolder()
        {
            initializer.Start();
            vnf.Register(nodeProvider);

            var result = vnf.GetChildren("/");

            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().Name, Is.EqualTo("upload"));
        }

        [Test]
        public void GetChildren_OfStartPage_DoesntInclude_UploadFolder()
        {
            initializer.Start();
            vnf.Register(nodeProvider);

            var result = vnf.GetChildren("/start/");

            Assert.That(result.Count(), Is.EqualTo(0));
        }

        [Test]
        public void GetChildren_OfUploadFolderPath_ReturnsChildren()
        {
            initializer.Start();
            vnf.Register(nodeProvider);

            var children = vnf.GetChildren("/upload/");

            Assert.That(children.Count(), Is.EqualTo(4));
        }

        [Test]
        public void GetChildren_OfUploadFolder_ReturnsChildren()
        {
            initializer.Start();
            vnf.Register(nodeProvider);

            var result = vnf.Get("/upload/");

            var children = result.GetChildPagesUnfiltered().Where(new NullFilter());//TODO: Not sure if this is correct.
            Assert.That(children.Count, Is.EqualTo(4));
            Assert.That(children.Any(c => c.Name == "Folder 2"));
            Assert.That(children.Any(c => c.Name == "Folder1"));
            Assert.That(children.Any(c => c.Name == "File.txt"));
            Assert.That(children.Any(c => c.Name == "Image.gif"));
        }

        [Test]
        public void Get_UploadFolder2()
        {
            fs.CreateDirectory("/upload2/");
            config.UploadFolders.Add("/upload2/");
            initializer = new VirtualFolderInitializer(host, persister, fs, vnf, monitor, new UploadFolderSource(host, config), nodeProvider);

            initializer.Start();
            vnf.Register(nodeProvider);

            var result = vnf.Get("/upload2/");

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo("upload2"));
        }

        [Test]
        public void GetChildren_Includes_UploadFolder2()
        {
            fs.CreateDirectory("/upload2/");
            config.UploadFolders.Add("/upload2/");
            initializer = new VirtualFolderInitializer(host, persister, fs, vnf, monitor, new UploadFolderSource(host, config), nodeProvider);

            initializer.Start();
            vnf.Register(nodeProvider);

            var result = vnf.GetChildren("/");

            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.Any(r => r.Name == "upload"));
            Assert.That(result.Any(r => r.Name == "upload2"));
        }

        // sites with upload folders

        [Test]
        public void Get_UploadFolder_GivesSiteUploadFolder()
        {
            fs.CreateDirectory("/siteupload/");
            var start2 = CreateOneItem<RootNode>(2, "start", root);
            var site = new Site(1, 2);
            site.UploadFolders.Add("/siteupload/");
            host.AddSites(new[] { site });
            initializer.Start();
            vnf.Register(nodeProvider);

            var defaultresult = vnf.Get("/siteupload/");
            var siteresult = vnf.Get("/start/siteupload/");

            Assert.That(defaultresult, Is.Null);
            Assert.That(siteresult, Is.Not.Null);
            Assert.That(siteresult.Name, Is.EqualTo("siteupload"));
        }

        [Test]
        public void GetChildren_OfSite_ReturnsSiteUploadFolders()
        {
            fs.CreateDirectory("/siteupload/");
            var start2 = CreateOneItem<RootNode>(2, "start", root);
            var site = new Site(1, 2);
            site.UploadFolders.Add("/siteupload/");
            host.AddSites(new[] { site });
            initializer.Start();
            vnf.Register(nodeProvider);

            var defaultresult = vnf.GetChildren("/");
            var siteresult = vnf.GetChildren("/start/");

            Assert.That(defaultresult.Any(c => c.Name == "siteupload"), Is.False);
            Assert.That(siteresult.Any(c => c.Name == "siteupload"), Is.True);
        }

        [Test]
        public void GetChildren_OfChild()
        {
            initializer.Start();
            vnf.Register(nodeProvider);

            var children = vnf.GetChildren("/upload/Folder1/");

            children.Single().Title.ShouldBe("File1.txt");
        }
    }
}
