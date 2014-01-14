using System.Collections;
using System.Web;
using N2.Configuration;
using N2.Tests.Fakes;
using N2.Web;
using NUnit.Framework;

namespace N2.Tests.Web.UrlParsing
{
    [TestFixture]
    public class CachingUrlParserTests : ParserTestsBase
    {
        new protected FakeRepository<ContentItem> repository;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            UrlParser inner = TestSupport.Setup(persister, wrapper, host);
            parser = new CachingUrlParserDecorator(inner, persister, wrapper, new CacheWrapper(persister, wrapper, new DatabaseSection()), new HostSection());
            CreateDefaultStructure();
            repository = (FakeRepository<ContentItem>) persister.Repository;
        }

        [TearDown]
        public override void TearDown()
        {
            foreach (DictionaryEntry entry in HttpRuntime.Cache)
            {
                HttpRuntime.Cache.Remove(entry.Key.ToString());
            }
            base.TearDown();
        }

        #region Forward to decorated tests
        [Test]
        public void WillForward_BuildUrl()
        {
            string url = parser.BuildUrl(page1);

            Assert.That(url, Is.EqualTo("/item1.aspx"));
        }

        [Test]
        public void WillForward_CurrentPage()
        {
            wrapper.CurrentPage = page1_1;
            var currentPage = parser.CurrentPage;

            Assert.That(currentPage, Is.EqualTo(page1_1));
        }

        [Test]
        public void WillForward_IsRootOrStartPage()
        {
            var isRootOrStartPage = parser.IsRootOrStartPage(startItem);

            Assert.That(isRootOrStartPage, Is.True);
        }

        [Test]
        public void WillForward_Parse()
        {
            var page = parser.Parse("/item1");

            Assert.That(page, Is.EqualTo(page1));
        }

        [Test]
        public void WillForward_ResolveTemplate()
        {
            var data = parser.FindPath("/item1");

            Assert.That(data.CurrentItem, Is.EqualTo(page1));
        }

        [Test]
        public void WillForward_StartPage()
        {
            var page = parser.StartPage;

            Assert.That(page, Is.EqualTo(startItem));
        }
        #endregion

        [Test]
        public void CanCache_ResolvedTemplate()
        {
            parser.FindPath("/item1/item1_1"); // find and cache

            var data = parser.FindPath("/item1/item1_1");

            var forcesLazyLoadButOtherwizeIgnored = data.CurrentPage;

            Assert.That(repository.LastOperation, Is.EqualTo("Get(3)"), "Should have loaded the parsed item directly.");
            Assert.That(data.CurrentItem, Is.EqualTo(page1_1));
        }

        [Test]
        public void ResolvedTemplateWithStartNode()
        {
            var data = parser.FindPath("/item1/item1_1", page1);

            Assert.That(repository.LastOperation, Is.EqualTo("Get(1)"), "Should have loaded the parsed item directly.");
            Assert.That(data.CurrentItem, Is.EqualTo(page1_1));
        }

        [Test]
        public void WillExpire_ResolvedTemplate_OnChanges()
        {
            parser.FindPath("/item1/item1_1"); // find and cache
            persister.Save(startItem); // incur changes

            var data = parser.FindPath("/item1/item1_1");

            Assert.That(repository.LastOperation, Is.EqualTo("Get(1)"), "Should have re-resolve template from start page.");
            Assert.That(data.CurrentItem, Is.EqualTo(page1_1));
        }
    }
}
