using N2.Configuration;
using N2.Web;
using NUnit.Framework;

namespace N2.Tests.Web
{
    [TestFixture]
    public class UrlParser_ParseDataTests : ParserTestsBase
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
        public void CanParseDataItemOnStartPage()
        {
            ContentItem parsedItem = parser.Parse("/?n2item=6");
            Assert.AreEqual(part1, parsedItem);
        }

        [Test]
        public void CanParseDataItemOneLevelDown()
        {
            ContentItem parsedItem = parser.Parse("/item2.aspx?n2item=7");
            Assert.AreEqual(part2, parsedItem);
        }

        [Test]
        public void CanParseDataItemTwoLevelsDown()
        {
            ContentItem parsedItem = parser.Parse("/item2/item2_1.aspx?n2item=8");
            Assert.AreEqual(part3, parsedItem);
        }

        [Test]
        public void ParseNonExistantDataItemReturnsPage()
        {
            ContentItem parsedItem = parser.Parse("/item2/item2_1.aspx?n2item=32");
            Assert.AreEqual(page2_1, parsedItem);
        }

        [Test]
        public void CanParseDataItemWithMixedCase()
        {
            ContentItem parsedItem = parser.Parse("/?N2iTeM=6");
            Assert.AreEqual(part1, parsedItem);
        }
    }
}
