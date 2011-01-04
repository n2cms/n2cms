using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Configuration;
using N2.Tests.Web.Items;
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

            string url = Url.Parse("http://www.n2cms.com/").PathAndQuery;
            Assert.AreEqual("/", url);
        }

        [Test]
        public void CanFind_SimpleUrl()
        {
            mocks.ReplayAll();

            string url = Url.Parse("http://www.n2cms.com/item1.aspx").PathAndQuery;
            Assert.AreEqual("/item1.aspx", url);
        }

        [Test]
        public void CanFind_UrlWithQueryString()
        {
            mocks.ReplayAll();

            string url = Url.Parse("http://www.n2cms.com/item1.aspx?item=1").PathAndQuery;
            Assert.AreEqual("/item1.aspx?item=1", url);
        }

        [Test]
        public void CanFind_DeepUrl_WithQueryStrings()
        {
            mocks.ReplayAll();

            string url = Url.Parse("http://www.n2cms.com/item1/item2/item3/item4.aspx?item=1&page=2&optional=yes").PathAndQuery;
            Assert.AreEqual("/item1/item2/item3/item4.aspx?item=1&page=2&optional=yes", url);
        }

        [Test]
        public void CanFind_UrlWithHash()
        {
            mocks.ReplayAll();

            string url = Url.Parse("http://www.n2cms.com/item1.aspx#theHash").PathAndQuery;
            Assert.AreEqual("/item1.aspx", url);
        }

        [Test]
        public void PagesOutsideStartPage_AreReferenced_ThroughTheirRewrittenUrl()
        {
            host = new Host(wrapper, 10, 1);
            parser = new UrlParser(persister, wrapper, host, new HostSection());

            CreateDefaultStructure();
            ContentItem root = CreateOneItem<PageItem>(10, "root", null);
            startItem.AddTo(root);
            ContentItem outside1 = CreateOneItem<PageItem>(11, "outside1", root);

            mocks.ReplayAll();

			Assert.AreEqual(parser.BuildUrl(root), root.FindPath(PathData.DefaultAction).RewrittenUrl.ToString());
			Assert.AreEqual(parser.BuildUrl(outside1), outside1.FindPath(PathData.DefaultAction).RewrittenUrl.ToString());
        }
    }
}
