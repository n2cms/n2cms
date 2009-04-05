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
			base.parser = parser = new UrlParser(persister, wrapper, notifier, host, new HostSection());
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
			Assert.AreEqual(item1, parsedItem);
		}

		[Test]
		public void CanParseItemTwoLevelsDown()
		{
			ContentItem parsedItem = parser.Parse("/item1/item1_1.aspx");
			Assert.AreEqual(item1_1, parsedItem);
		}

		[Test]
		public void CanParseItemOneStepOneLevelDown()
		{
			ContentItem parsedItem = parser.Parse("/item2.aspx");
			Assert.AreEqual(item2, parsedItem);
		}

		[Test]
		public void CanParseItemOneStepTwoLevelsDown()
		{
			ContentItem parsedItem = parser.Parse("/item2/item2_1.aspx");
			Assert.AreEqual(item2_1, parsedItem);
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
			Assert.AreEqual(item2_1, parsedItem);
		}

		[Test]
		public void CanParseItemWithHash()
		{
			ContentItem parsedItem = parser.Parse("/item1.aspx#someHash");
			Assert.AreEqual(item1, parsedItem);
		}

		[Test]
		public void CanParse_StartPage()
		{
			ContentItem parsedItem = parser.Parse("/default.aspx");
			Assert.AreEqual(startItem, parsedItem);
		}

		[Test]
		public void CanParse_StartPage2()
		{
			ContentItem parsedItem = parser.Parse("/default.aspx?");
			Assert.AreEqual(startItem, parsedItem);
		}
	}
}
