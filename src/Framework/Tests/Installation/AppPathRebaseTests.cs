using System.Collections.Generic;
using System.Linq;
using N2.Details;
using N2.Edit.Installation;
using N2.Persistence;
using N2.Persistence.Finder;
using N2.Web;
using NUnit.Framework;
using Rhino.Mocks;

namespace N2.Tests.Installation
{
    #region RelativityItem
    public class RelativityItem : ContentItem
    {
        [EditableFileUpload]
        public string FileUrl { get; set; }

        [EditableImage]
        public string ImageUrl { get; set; }

        [EditableLink]
        public string LinkUrl { get; set; }

        [EditableUrl]
        public string SomeUrl { get; set; }

        [EditableFreeTextArea]
        public string Text { get; set; }

        [EditableText]
        public string Heading { get; set; }
    }

    public class InheritsRelativityItem : RelativityItem
    {
    }
    
    #endregion

    [TestFixture]
    public class AppPathRebaseTests
    {
        RelativityItem item;

        [SetUp]
        public void SetUp()
        {
            item = new RelativityItem();
        }

        // / -> /app/

        [Test]
        public void Rebase_EditableFileUpload_FromSite_ToApplication()
        {
            item.FileUrl = "/Upload/hello.jpg";

            AppPathRebaser.Rebase(item, "/", "/app/");

            Assert.That(item.FileUrl, Is.EqualTo("/app/Upload/hello.jpg"));
        }

        [Test]
        public void Rebase_EditableImage_FromSite_ToApplication()
        {
            item.ImageUrl = "/Upload/hello.jpg";

            AppPathRebaser.Rebase(item, "/", "/app/");

            Assert.That(item.ImageUrl, Is.EqualTo("/app/Upload/hello.jpg"));
        }

        [Test]
        public void Rebase_EditableLink_FromSite_ToApplication()
        {
            item.LinkUrl = "/hello/world/";

            AppPathRebaser.Rebase(item, "/", "/app/");

            Assert.That(item.LinkUrl, Is.EqualTo("/app/hello/world/"));
        }

        [Test]
        public void Rebase_EditableUrl_FromSite_ToApplication()
        {
            item.SomeUrl = "/hello/world/";

            AppPathRebaser.Rebase(item, "/", "/app/");

            Assert.That(item.SomeUrl, Is.EqualTo("/app/hello/world/"));
        }

        [Test]
        public void Rebase_AnchorWithin_EditableFreeTextArea_FromSite_ToApplication()
        {
            item.Text = @"<a href=""/hello/world/"">hello</a>";

            AppPathRebaser.Rebase(item, "/", "/app/");

            Assert.That(item.Text, Is.EqualTo(@"<a href=""/app/hello/world/"">hello</a>"));
        }

        [Test]
        public void Rebase_ImageWithin_EditableFreeTextArea_FromSite_ToApplication()
        {
            item.Text = @"<img src=""/Upload/hello.jpg""/>";
            
            AppPathRebaser.Rebase(item, "/", "/app/");

            Assert.That(item.Text, Is.EqualTo(@"<img src=""/app/Upload/hello.jpg""/>"));
        }

        [Test]
        public void Ignores_TextFields_WhenRebasing_FromSite_ToApplication()
        {
            item.Heading = "This is a title";

            AppPathRebaser.Rebase(item, "/", "/app/");

            Assert.That(item.Heading, Is.EqualTo("This is a title"));
        }

        [Test]
        public void Ignores_TextFields_WithPaths_WhenRebasing_FromSite_ToApplication()
        {
            item.Heading = "This is a title";

            AppPathRebaser.Rebase(item, "/", "/app/");

            Assert.That(item.Heading, Is.EqualTo("This is a title"));
        }

        [Test]
        public void Ignores_NonLinks_WhenRebasing_FromSite_ToApplication()
        {
            item.Text = "<p>This is an url: /hello/world/</p>";

            AppPathRebaser.Rebase(item, "/", "/app/");

            Assert.That(item.Text, Is.EqualTo("<p>This is an url: /hello/world/</p>"));
        }

        [Test]
        public void Rebase_MultipleLinks_InEditableFreeTextArea_FromSite_ToApplication()
        {
            item.Text = @"<p><a href=""/hello/"">hello</a></p>
<p><a href=""/world/"">hello</a><a href=""/hello/world/"">hello</a></p>";

            AppPathRebaser.Rebase(item, "/", "/app/");

            Assert.That(item.Text, Is.EqualTo(@"<p><a href=""/app/hello/"">hello</a></p>
<p><a href=""/app/world/"">hello</a><a href=""/app/hello/world/"">hello</a></p>"));
        }

        [Test]
        public void Rebase_ImageWithin_EditableFreeTextArea_InheritedFromBaseClass_FromSite_ToApplication()
        {
            item = new InheritsRelativityItem();
            item.Text = @"<img src=""/Upload/hello.jpg""/>";

            AppPathRebaser.Rebase(item, "/", "/app/");

            Assert.That(item.Text, Is.EqualTo(@"<img src=""/app/Upload/hello.jpg""/>"));
        }

        [Test]
        public void ExternalUrl_IsNotRebased()
        {
            item.Text = @"<p><a href=""http://n2cms.com/"">hello</a></p>";

            AppPathRebaser.Rebase(item, "/", "/app/");

            Assert.That(item.Text, Is.EqualTo(@"<p><a href=""http://n2cms.com/"">hello</a></p>"));
        }

        // /app/ -> /

        [Test]
        public void Rebase_EditableFileUpload_FromApplication_ToSite()
        {
            item.FileUrl = "/app/Upload/hello.jpg";

            AppPathRebaser.Rebase(item, "/app/", "/");

            Assert.That(item.FileUrl, Is.EqualTo("/Upload/hello.jpg"));
        }

        [Test]
        public void Rebase_EditableImage_FromApplication_ToSite()
        {
            item.ImageUrl = "/app/Upload/hello.jpg";

            AppPathRebaser.Rebase(item, "/app/", "/");

            Assert.That(item.ImageUrl, Is.EqualTo("/Upload/hello.jpg"));
        }

        [Test]
        public void Rebase_EditableLink_FromApplication_ToSite()
        {
            item.LinkUrl = "/app/hello/world/";

            AppPathRebaser.Rebase(item, "/app/", "/");

            Assert.That(item.LinkUrl, Is.EqualTo("/hello/world/"));
        }

        [Test]
        public void Rebase_EditableUrl_FromApplication_ToSite()
        {
            item.SomeUrl = "/app/hello/world/";

            AppPathRebaser.Rebase(item, "/app/", "/");

            Assert.That(item.SomeUrl, Is.EqualTo("/hello/world/"));
        }

        [Test]
        public void Rebase_AnchorWithin_EditableFreeTextArea_FromApplication_ToSite()
        {
            item.Text = @"<a href=""/app/hello/world/"">hello</a>";

            AppPathRebaser.Rebase(item, "/app/", "/");

            Assert.That(item.Text, Is.EqualTo(@"<a href=""/hello/world/"">hello</a>"));
        }

        [Test]
        public void Rebase_ImageWithin_EditableFreeTextArea_FromApplication_ToSite()
        {
            item.Text = @"<img src=""/app/Upload/hello.jpg""/>";

            AppPathRebaser.Rebase(item, "/app/", "/");

            Assert.That(item.Text, Is.EqualTo(@"<img src=""/Upload/hello.jpg""/>"));
        }

        [Test]
        public void Ignores_TextFields_WhenRebasing_FromApplication_ToSite()
        {
            item.Heading = "This is a title";

            AppPathRebaser.Rebase(item, "/app/", "/");

            Assert.That(item.Heading, Is.EqualTo("This is a title"));
        }

        [Test]
        public void Ignores_NonLinks_WhenRebasing_FromApplication_ToSite()
        {
            item.Text = "<p>This is an url: /app/hello/world/</p>";

            AppPathRebaser.Rebase(item, "/app/", "/");

            Assert.That(item.Text, Is.EqualTo("<p>This is an url: /app/hello/world/</p>"));
        }

        // rebase items

        [Test]
        public void AllItems_AreRebased()
        {
            var root = new RelativityItem();
            AppPathRebaser rebaser = GetRebaser(root);
            
            root.ImageUrl = "/upload/hello.jpg";
            item.ImageUrl = "/upload/hello.jpg";

            rebaser.Rebase("/", "/app/").ToList();

            Assert.That(root.ImageUrl, Is.EqualTo("/app/upload/hello.jpg"));
            Assert.That(item.ImageUrl, Is.EqualTo("/app/upload/hello.jpg"));
        }

        [Test]
        public void NewPath_IsStored_OnRoot()
        {
            var root = new RelativityItem();
            AppPathRebaser rebaser = GetRebaser(root);

            rebaser.Rebase("/", "/app/").ToList();

            Assert.That(root[InstallationManager.InstallationAppPath], Is.EqualTo("/app/"));
        }

        private AppPathRebaser GetRebaser(RelativityItem root)
        {
            var items = new List<ContentItem> { root, item };

            var finder = MockRepository.GenerateStub<IItemFinder>();
            finder.Stub(f => f.All.Select()).IgnoreArguments().Return(items);

            var persister = MockRepository.GenerateStub<IPersister>();
            persister.Stub(p => p.Repository.BeginTransaction()).Return(MockRepository.GenerateStub<ITransaction>());
            persister.Stub(p => p.Get(1)).Return(root);

            var host = MockRepository.GenerateStub<IHost>();
            host.DefaultSite = new Site(1);
            return new AppPathRebaser(finder, persister, host);
        }
    }
}
