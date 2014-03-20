using N2.Web;
using NUnit.Framework;

namespace N2.Tests.Web
{
    [TestFixture]
    public class WhileParsingUrlsWhenMultipleSites : MultipleHostUrlParserTests
    {
        [Test]
        public void CanGetFirstSite()
        {
            mocks.ReplayAll();

            Site s = host.GetSite("http://www.n2cms.com");
            Assert.AreSame(sites[1], s);
        }

        [Test]
        public void CanGetSiteWithPort()
        {
            mocks.ReplayAll();

            Site s = host.GetSite("http://www.n2cms.com:8080");
            Assert.AreSame(sites[3], s);
        }


        [Test]
        public void CanParseStartPageUrl()
        {
            CreateItemsAndBuildExpectations("www.n2cms.com", "/");

            Assert.AreSame(page1, parser.Parse("/"));
        }

        [Test]
        public void CanParseSubPageUrl()
        {
            CreateItemsAndBuildExpectations("www.n2cms.com", "/item1_1.aspx");

            Assert.AreSame(page1_1, parser.Parse("/item1_1.aspx"));
        }

        [Test]
        public void CanParseSiteWithPortStartPage()
        {
            CreateItemsAndBuildExpectations("www.n2cms.com:8080", "/");

            Assert.AreSame(page2_1, parser.Parse("/"));
        }

        [Test]
        public void CanParseStartPageOnOtherSite()
        {
            CreateItemsAndBuildExpectations("www.n2cms.com:8080", "/");

            Assert.AreSame(page1, parser.Parse("http://www.n2cms.com/"));
        }

        [Test]
        public void CanParseSubPageOnOtherSite()
        {
            CreateItemsAndBuildExpectations("www.n2cms.com:8080", "/item1_1.aspx");

            Assert.AreSame(page1_1, parser.Parse("http://www.n2cms.com/item1_1.aspx"));
        }

        [Test]
        public void CanParseDataItem()
        {
            CreateItemsAndBuildExpectations("n2.libardo.com", "/item2_1.aspx?n2item=7");

            Assert.AreSame(part2, parser.Parse("/item2_1.aspx?n2item=7"));
        }

        [Test]
        public void ParsePage_WithNonInteger_PageQueryString_ReturnsNull()
        {
            CreateItemsAndBuildExpectations("n2.libardo.com", "http://www.externalsite.com/somepage.html?n2item=this_isnt_a_number");

            Assert.IsNull(parser.Parse("http://www.externalsite.com/somepage.html?n2item=this_isnt_a_number"));
        }

        [Test]
        public void ParsePage_WithNonInteger_ItemQueryString_ReturnsNull()
        {
            CreateItemsAndBuildExpectations("n2.libardo.com", "http://www.externalsite.com/somepage.html?n2item=this_isnt_a_number");

            Assert.IsNull(parser.Parse("http://www.externalsite.com/somepage.html?n2item=this_isnt_a_number"));
        }

        [Test]
        public void ParsePage_DoesntThrowException_WhenGivenEmail()
        {
            var item = parser.Parse("mailto:cristian@somesite.com");
            Assert.That(item, Is.Null);
        }

        private void CreateItemsAndBuildExpectations(string host, string url)
        {
            CreateDefaultStructure();
            wrapper.Url = "http://" + host + "/";
            mocks.ReplayAll();
        }
    }
}
