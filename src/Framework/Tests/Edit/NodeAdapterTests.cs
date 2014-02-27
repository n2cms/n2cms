using System.Linq;
using N2.Edit;
using N2.Edit.Workflow;
using N2.Security;
using N2.Tests.Fakes;
using N2.Web;
using N2.Edit.Settings;
using NUnit.Framework;

namespace N2.Tests.Edit
{
    public class NodeAdapterTests : N2.Tests.ItemPersistenceMockingBase
    {
        NodeAdapter adapter;
        FakeMemoryFileSystem fs;

        ContentItem root;
        ContentItem start;
        ContentItem part;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            root = CreateOneItem<Items.NormalPage>(1, "root", null);
            start = CreateOneItem<Items.NormalPage>(2, "start", root);
            part = CreateOneItem<Items.NormalItem>(3, "part", root);
            part.ZoneName = "Zone";

            adapter = new NodeAdapter();
            adapter.ManagementPaths = new EditUrlManager(null, new N2.Configuration.EditSection());
            adapter.FileSystem = fs = new FakeMemoryFileSystem();
            adapter.NodeFactory = new VirtualNodeFactory();
            adapter.WebContext = new Fakes.FakeWebContextWrapper();
            adapter.Security = new SecurityManager(adapter.WebContext, new N2.Configuration.EditSection());
            adapter.Host = new Host(null, root.ID, start.ID);
            adapter.Settings = new FakeNavigationSettings();
            adapter.Sources = TestSupport.SetupContentSource(adapter.WebContext, adapter.Host, persister.Repository);
        }

        [Test]
        public void Parts_AreNotReturned()
        {
            var children = adapter.GetChildren(root, Interfaces.Managing);
            Assert.That(children.Single(), Is.EqualTo(start));
        }

        [Test]
        public void Parts_AreReturned_WhenDisplayDataItems()
        {
            adapter.Settings.DisplayDataItems = true;
            var children = adapter.GetChildren(root, Interfaces.Managing);
            Assert.That(children, Is.EquivalentTo(new [] { start, part }));
        }

        [Test]
        public void VirtualNodes_AreAdded_ToTheirParent()
        {
            adapter.NodeFactory.Register(new FunctionalNodeProvider("/hello/", (path) => new Items.NormalPage { Name = "World", Parent = root }));

            var children = adapter.GetChildren(root, Interfaces.Managing);
            Assert.That(children.Count(), Is.EqualTo(1 + root.Children.FindPages().Count()));
            Assert.That(children.Any(c => c.Name == "World"));
        }

        [Test]
        public void VirtualNodes_AreNotAdded_ToOtherNodes()
        {
            adapter.NodeFactory.Register(new FunctionalNodeProvider("/hello/", (path) => new Items.NormalPage { Name = "World", Parent = start }));

            var children = adapter.GetChildren(start, Interfaces.Managing);
            Assert.That(children.Count(), Is.EqualTo(start.Children.Count));
            Assert.That(children.Any(c => c.Name == "World"), Is.False);
        }

        [Test]
        public void VirtualNodes_CanBeAdded_ToADeeperLevel()
        {
            adapter.NodeFactory.Register(new FunctionalNodeProvider("/start/hello/", (path) => new Items.NormalPage { Name = "World", Parent = start }));

            var children = adapter.GetChildren(start, Interfaces.Managing);
            Assert.That(children.Count(), Is.EqualTo(1 + start.Children.Count));
            Assert.That(children.Any(c => c.Name == "World"));
        }

        [Test]
        public void GlobalUploadFolders_AreNotAdded()
        {
            fs.CreateDirectory("~/upload/");

            var uploads = adapter.GetUploadDirectories(adapter.Host.DefaultSite);
            Assert.That(uploads.Count(), Is.EqualTo(0));
        }

        [Test]
        public void SiteUploadFolders_AreAdded()
        {
            fs.CreateDirectory("~/upload/");
            fs.CreateDirectory("~/siteupload/");
            adapter.Host.DefaultSite.UploadFolders.Add("~/siteupload/");

            var uploads = adapter.GetUploadDirectories(adapter.Host.DefaultSite);

            Assert.That(uploads.Count(), Is.EqualTo(1));
            Assert.That(uploads.Any(u => u.VirtualPath == "~/siteupload/"));
        }

        public class FakeNavigationSettings : NavigationSettings
        {
            public FakeNavigationSettings()
                : base(null)
            {
            }

            bool displayDataItems;

            public override bool DisplayDataItems
            {
                get { return displayDataItems; }
                set { displayDataItems = value; }
            }
        }
    }
}
