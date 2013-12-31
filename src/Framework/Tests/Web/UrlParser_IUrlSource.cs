using N2.Configuration;
using N2.Tests.Web.Items;
using N2.Web;
using NUnit.Framework;
using Shouldly;
using N2.Engine;

namespace N2.Tests.Web
{
    [TestFixture]
    public class UrlParser_IUrlSource : ParserTestsBase<Items.UrlSourcePage, DataItem>
    {
        protected new UrlParser parser;
        private DirectUrlInjector injector;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            base.parser = parser = TestSupport.Setup(persister, wrapper, host);
            injector = new DirectUrlInjector(host, parser, repository, TestSupport.SetupDefinitions(typeof(UrlSourcePage), typeof(DataItem)));
            injector.Start();
            CreateDefaultStructure();
            Url.DefaultExtension = "/";
        }

        [TearDown]
        public override void TearDown()
        {
            Url.DefaultExtension = ".aspx";
            base.TearDown();
        }

        [Test]
        public void UrlSource_OvertaksUrlGeneration()
        {
            page1.DirectUrl = "/hello";
            
            string url = parser.BuildUrl(page1);

            url.ShouldBe("/hello/");
        }

        [Test]
        public void EmptyUrlSource_FallbacksTo_NormalUrlGeneration()
        {
            string url = parser.BuildUrl(page1);

            url.ShouldBe("/item1/");
        }

        [Test]
        public void UrlSource_AffectsChildren()
        {
            page1.DirectUrl = "/hello";

            string url = parser.BuildUrl(page1_1);

            url.ShouldBe("/hello/item1_1/");
        }

        [Test]
        public void UrlSource_OvertaksParentPath()
        {
            page1_1.DirectUrl = "/hello";

            string url = parser.BuildUrl(page1_1);

            url.ShouldBe("/hello/");
        }

        [Test]
        public void FindPath_UsingUrlSource()
        {
            page1_1.DirectUrl = "/hello";
            repository.Save(page1_1);

            var path = parser.FindPath("/hello/");

            path.CurrentItem.ShouldBe(page1_1);
        }

        [Test]
        public void Parse_UsingUrlSource()
        {
            page1_1.DirectUrl = "/hello";
            repository.Save(page1_1);

            var item = parser.Parse("/hello/");

            item.ShouldBe(page1_1);
        }

        [Test]
        public void FindPath_UsingUrlSource_OnParent()
        {
            page1.DirectUrl = "/hello";
            repository.Save(page1);

            var path = parser.FindPath("/hello/item1_1/");

            path.CurrentItem.ShouldBe(page1_1);
        }

        [Test]
        public void Parse_UsingUrlSource_OnParent()
        {
            page1.DirectUrl = "/hello";
            repository.Save(page1);

            var item = parser.Parse("/hello/item1_1/");

            item.ShouldBe(page1_1);
        }

        [Test]
        public void DeletedPages_AreNotFound_ViaDirectUrl()
        {
            page1.DirectUrl = "/hello";
            page1.State = ContentState.Deleted;
            repository.Save(page1);

            var item = parser.Parse("/hello/");

            item.ShouldBe(null);
        }
    }
}
