using N2.Configuration;
using N2.Web;
using NUnit.Framework;

namespace N2.Tests.Web
{
    [TestFixture]
    public class UrlParser_ParsePageTests : ParserTestsBase
    {
        protected new UrlParser parser;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            base.parser = parser = TestSupport.Setup(persister, wrapper, host);
            CreateDefaultStructure();
        }

        [Test]
        public void CanParseStartPageUrl()
        {
            ContentItem parsedItem = parser.Parse("/");
            Assert.AreEqual(startItem, parsedItem);
        }

        [Test]
        public void CanParseItemOneLevelDown()
        {
            ContentItem parsedItem = parser.Parse("/item1.aspx");
            Assert.AreEqual(page1, parsedItem);
        }

        [Test]
        public void CanParseItemTwoLevelsDown()
        {
            ContentItem parsedItem = parser.Parse("/item1/item1_1.aspx");
            Assert.AreEqual(page1_1, parsedItem);
        }

        [Test]
        public void CanParseItemOneStepOneLevelDown()
        {
            ContentItem parsedItem = parser.Parse("/item2.aspx");
            Assert.AreEqual(page2, parsedItem);
        }

        [Test]
        public void CanParseItemOneStepTwoLevelsDown()
        {
            ContentItem parsedItem = parser.Parse("/item2/item2_1.aspx");
            Assert.AreEqual(page2_1, parsedItem);
        }

        [Test]
        public void ParseNonExistantItemYeldsNull()
        {
            ContentItem parsedItem = parser.Parse("/item3.aspx");
            Assert.IsNull(parsedItem);
        }

        [Test]
        public void CanParseItemWithMixedCase()
        {
            ContentItem parsedItem = parser.Parse("/iTeM2/ItEm2_1.AsPx");
            Assert.AreEqual(page2_1, parsedItem);
        }

        [Test]
        public void CanParseItemWithHash()
        {
            ContentItem parsedItem = parser.Parse("/item1.aspx#someHash");
            Assert.AreEqual(page1, parsedItem);
        }

        [Test]
        public void CanParse_StartPage()
        {
            ContentItem parsedItem = parser.Parse("/Default.aspx");
            Assert.AreEqual(startItem, parsedItem);
        }
    }
}
