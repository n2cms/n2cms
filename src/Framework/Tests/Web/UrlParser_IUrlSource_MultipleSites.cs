using N2.Configuration;
using N2.Tests.Web.Items;
using N2.Web;
using NUnit.Framework;
using Shouldly;
using N2.Engine;

namespace N2.Tests.Web
{
    [TestFixture]
    public class UrlParser_IUrlSource_MultipleSites : ParserTestsBase<Items.UrlSourcePage, DataItem>
    {
        protected new MultipleSitesParser parser;
        private DirectUrlInjector injector;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            CreateDefaultStructure();

            host.ReplaceSites(host.DefaultSite, new Site[] { host.DefaultSite, new Site(1, 2, "n2cms.com"), new Site(1, 4, "libardo.com") });

            base.parser = parser = new MultipleSitesParser(persister, wrapper, host, new N2.Plugin.ConnectionMonitor(), new HostSection());
            injector = new DirectUrlInjector(host, parser, repository, TestSupport.SetupDefinitions(typeof(UrlSourcePage), typeof(DataItem)));
            injector.Start();
            Url.DefaultExtension = "/";
        }

        [TearDown]
        public override void TearDown()
        {
            Url.DefaultExtension = ".aspx";
            base.TearDown();
        }

        [Test]
        public void UrlSource_OvertaksUrlGeneration_ForOtherSites()
        {
            page2_1.DirectUrl = "/hello";
            
            string url = parser.BuildUrl(page2_1);

            url.ShouldBe("http://libardo.com/hello/");
        }

        [Test]
        public void EmptyUrlSource_FallbacksTo_NormalUrlGeneration()
        {
            string url = parser.BuildUrl(page2_1);

            url.ShouldBe("http://libardo.com/item2_1/");
        }

        [Test]
        public void UrlSource_AffectsChildren()
        {
            page2_1.DirectUrl = "/hello";
            var sub = CreateOneItem<PageItem>(0, "sub", page2_1);

            string url = parser.BuildUrl(sub);

            url.ShouldBe("http://libardo.com/hello/sub/");
        }

        [Test]
        public void UrlSource_OvertaksParentPath()
        {
            var sub = CreateOneItem<UrlSourcePage>(0, "sub", page2_1);
            sub.DirectUrl = "/hello";

            string url = parser.BuildUrl(sub);

            url.ShouldBe("http://libardo.com/hello/");
        }

        [Test]
        public void FindPath_UsingUrlSource()
        {
            page2_1.DirectUrl = "/hello";
            repository.Save(page1_1);

            var path = parser.FindPath("http://libardo.com/hello/");

            path.CurrentItem.ShouldBe(page2_1);
        }

        [Test]
        public void FindPath_UsingUrlSource_DoesntFindOnOtherSites()
        {
            page2_1.DirectUrl = "/hello";
            repository.Save(page2_1);

            var path = parser.FindPath("http://n2cms.com/hello/");

            path.CurrentItem.ShouldBe(null);
        }

        [Test]
        public void FindPath_UsingUrlSource_UsesTheUrl_ToFindSite()
        {
            page1_1.DirectUrl = "/hello";
            repository.Save(page1_1);
            page2_1.DirectUrl = "/hello";
            repository.Save(page2_1);

            parser.FindPath("http://n2cms.com/hello/").CurrentItem.ShouldBe(page1_1);
            parser.FindPath("http://libardo.com/hello/").CurrentItem.ShouldBe(page2_1);
        }

        [Test]
        public void Parse_UsingUrlSource()
        {
            page2_1.DirectUrl = "/hello";
            repository.Save(page1_1);

            var item = parser.Parse("http://libardo.com/hello/");

            item.ShouldBe(page2_1);
        }

        [Test]
        public void Parse_UsingUrlSource_DoesntFindOnOtherSites()
        {
            page2_1.DirectUrl = "/hello";
            repository.Save(page2_1);

            var item = parser.Parse("http://n2cms.com/hello/");

            item.ShouldBe(null);
        }

        [Test]
        public void Parse_UsingUrlSource_UsesTheUrl_ToFindSite()
        {
            page1_1.DirectUrl = "/hello";
            repository.Save(page1_1);
            page2_1.DirectUrl = "/hello";
            repository.Save(page2_1);

            parser.Parse("http://n2cms.com/hello/").ShouldBe(page1_1);
            parser.Parse("http://libardo.com/hello/").ShouldBe(page2_1);
        }

        [Test]
        public void FindPath_UsingUrlSource_OnParent()
        {
            page2_1.DirectUrl = "/hello";
            repository.Save(page2_1);

            var sub = CreateOneItem<PageItem>(0, "sub", page2_1);

            var path = parser.FindPath("http://libardo.com/hello/sub/");

            path.CurrentItem.ShouldBe(sub);
        }

        [Test]
        public void Parse_UsingUrlSource_OnParent()
        {
            page2_1.DirectUrl = "/hello";
            repository.Save(page2_1);

            var sub = CreateOneItem<PageItem>(0, "sub", page2_1);

            var item = parser.Parse("http://libardo.com/hello/sub/");

            item.ShouldBe(sub);
        }
    }
}
