using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using N2.Persistence;
using N2.Web;

namespace N2.Tests.Web
{
    [TestFixture]
    public class UrlParserPathAndQueryTests : ParserTestsBase
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
        }

        [Test]
        public void CanFind_RootUrl()
        {
            mocks.ReplayAll();

            string url = parser.GetPathAndQuery("http://www.n2cms.com/");
            Assert.AreEqual("/", url);
        }

        [Test]
        public void CanFind_SimpleUrl()
        {
            mocks.ReplayAll();

            string url = parser.GetPathAndQuery("http://www.n2cms.com/item1.aspx");
            Assert.AreEqual("/item1.aspx", url);
        }

        [Test]
        public void CanFind_UrlWithQueryString()
        {
            mocks.ReplayAll();

            string url = parser.GetPathAndQuery("http://www.n2cms.com/item1.aspx?item=1");
            Assert.AreEqual("/item1.aspx?item=1", url);
        }

        [Test]
        public void CanFind_DeepUrl_WithQueryStrings()
        {
            mocks.ReplayAll();

            string url = parser.GetPathAndQuery("http://www.n2cms.com/item1/item2/item3/item4.aspx?item=1&page=2&optional=yes");
            Assert.AreEqual("/item1/item2/item3/item4.aspx?item=1&page=2&optional=yes", url);
        }

        [Test]
        public void CanFind_UrlWithHash()
        {
            mocks.ReplayAll();

            string url = parser.GetPathAndQuery("http://www.n2cms.com/item1.aspx#theHash");
            Assert.AreEqual("/item1.aspx#theHash", url);
        }

        [Test]
        public void PagesOutsideStartPage_AreReferenced_ThroughTheirRewrittenUrl()
        {
            notifier = mocks.Stub<IItemNotifier>();
            host = new Host(null, 10, 1);
            parser = new UrlParser(persister, wrapper, notifier, host);

            CreateDefaultStructure();
            ContentItem root = CreateOneItem<PageItem>(10, "root", null);
            startItem.AddTo(root);
            ContentItem outside1 = CreateOneItem<PageItem>(11, "outside1", root);

            mocks.ReplayAll();

            Assert.AreEqual(parser.BuildUrl(root), root.RewrittenUrl);
            Assert.AreEqual(parser.BuildUrl(outside1), outside1.RewrittenUrl);
        }
    }
}
