using N2.Configuration;
using N2.Edit.FileSystem;
using N2.Web;
using NUnit.Framework;

namespace N2.Tests.Edit.FileSystem
{
    [TestFixture]
    public class DefaultFolderFinderTest : ItemTestsBase
    {
        EditSection config;
        Host host;
        ContentItem root;
        ContentItem item1;
        ContentItem item2;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            host = new Host(new Fakes.FakeWebContextWrapper(), 1, 1);
            config = new EditSection();
            config.DefaultDirectory = new DefaultDirectoryElement();
            config.UploadFolders = new RootFileSystemFolderCollection();

            root = CreateOneItem<Items.NormalPage>(1, "root", null);
            item1 = CreateOneItem<Items.NormalPage>(2, "item1", root);
            item2 = CreateOneItem<Items.NormalPage>(3, "item2", item1);
        }

        [TestCase(DefaultDirectoryMode.RecursiveNames, "~/upload/item1/item2/")]
        [TestCase(DefaultDirectoryMode.TopNodeName, "~/upload/item1/")]
        [TestCase(DefaultDirectoryMode.NodeName, "~/upload/item2/")]
        [TestCase(DefaultDirectoryMode.UploadFolder, "~/upload/")]
        [TestCase(DefaultDirectoryMode.RecursiveNamesFromParent, "~/upload/item1/")]
        public void CanFind_ShadowFolder_WithoutDefaultFolderRoot(DefaultDirectoryMode stragegy, string expectedPath)
        {
            config.DefaultDirectory.Mode = stragegy;
            DefaultDirectorySelector finder = new DefaultDirectorySelector(host, config);

            string path = finder.GetDefaultDirectory(item2);

            Assert.That(path, Is.EqualTo(expectedPath));
        }

        [TestCase(DefaultDirectoryMode.RecursiveNames, "~/DefaultFolder/item1/item2/")]
        [TestCase(DefaultDirectoryMode.TopNodeName, "~/DefaultFolder/item1/")]
        [TestCase(DefaultDirectoryMode.NodeName, "~/DefaultFolder/item2/")]
        [TestCase(DefaultDirectoryMode.UploadFolder, "~/DefaultFolder/")]
        [TestCase(DefaultDirectoryMode.RecursiveNamesFromParent, "~/DefaultFolder/item1/")]
        public void CanFind_ShadowFolder_WithDefaultFolderRoot(DefaultDirectoryMode strategy, string expectedPath)
        {
            config.DefaultDirectory.Mode = strategy;
            config.DefaultDirectory.RootPath = "~/DefaultFolder/";
            DefaultDirectorySelector finder = new DefaultDirectorySelector(host, config);

            string path = finder.GetDefaultDirectory(item2);

            Assert.That(path, Is.EqualTo(expectedPath), "Strategy " + strategy);
        }

        [Test]
        public void CanFind_ShadowFolder_WhenUploading_NewFile()
        {
            config.DefaultDirectory.Mode = DefaultDirectoryMode.RecursiveNamesFromParent;
            config.DefaultDirectory.RootPath = "~/DefaultFolder/";
            DefaultDirectorySelector finder = new DefaultDirectorySelector(host, config);

            var item3 = CreateOneItem<Items.NormalPage>(4, "item3", null);
            item3.VersionOf = item2;
            string path = finder.GetDefaultDirectory(item3);

            Assert.That(path, Is.EqualTo("~/DefaultFolder/item1/"), "Strategy ~/DefaultFolder/item1/");
        }
    }
}
