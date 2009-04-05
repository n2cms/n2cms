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
			base.parser = parser = new UrlParser(persister, wrapper, notifier, host, new HostSection());
			CreateDefaultStructure();
		}

		[Test]
		public void CanParseDataItemOnStartPage()
		{
			ContentItem parsedItem = parser.Parse("/?item=6");
			Assert.AreEqual(data1, parsedItem);
		}

		[Test]
		public void CanParseDataItemOneLevelDown()
		{
			ContentItem parsedItem = parser.Parse("/item2.aspx?item=7");
			Assert.AreEqual(data2, parsedItem);
		}

		[Test]
		public void CanParseDataItemTwoLevelsDown()
		{
			ContentItem parsedItem = parser.Parse("/item2/item2_1.aspx?item=8");
			Assert.AreEqual(data3, parsedItem);
		}

		[Test]
		public void ParseNonExistantDataItemReturnsPage()
		{
			ContentItem parsedItem = parser.Parse("/item2/item2_1.aspx?item=32");
			Assert.AreEqual(item2_1, parsedItem);
		}

		[Test]
		public void CanParseDataItemWithMixedCase()
		{
			ContentItem parsedItem = parser.Parse("/?iTeM=6");
			Assert.AreEqual(data1, parsedItem);
		}
	}
}