using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;
using Rhino.Mocks;
using N2.Persistence;
using N2.Web;

namespace N2.Tests.Web
{
	[TestFixture]
	public class UrlParserTests : ParserTestsBase
	{
		#region SetUp

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();
			parser = new DefaultUrlParser(persister, wrapper, notifier, site);
		}

		#endregion

		#region Parse Page Tests
		[Test]
		public void CanParseStartPageUrl()
		{
			CreateItems(true);

			ContentItem parsedItem = parser.Parse("/");
			Assert.AreEqual(startItem, parsedItem);
		}

		[Test]
		public void CanParseItemOneLevelDown()
		{
			CreateItems(true);

			ContentItem parsedItem = parser.Parse("/item1.aspx");
			Assert.AreEqual(item1, parsedItem);
		}

		[Test]
		public void CanParseItemTwoLevelsDown()
		{
			CreateItems(true);

			ContentItem parsedItem = parser.Parse("/item1/item1_1.aspx");
			Assert.AreEqual(item1_1, parsedItem);
		}

		[Test]
		public void CanParseItemOneStepOneLevelDown()
		{
			CreateItems(true);

			ContentItem parsedItem = parser.Parse("/item2.aspx");
			Assert.AreEqual(item2, parsedItem);
		}

		[Test]
		public void CanParseItemOneStepTwoLevelsDown()
		{
			CreateItems(true);

			ContentItem parsedItem = parser.Parse("/item2/item2_1.aspx");
			Assert.AreEqual(item2_1, parsedItem);
		}

		[Test]
		public void ParseNonExistantItemYeldsNull()
		{
			CreateItems(true);

			ContentItem parsedItem = parser.Parse("/item3.aspx");
			Assert.IsNull(parsedItem);
		}

		[Test]
		public void CanParseItemWithMixedCase()
		{
			CreateItems(true);

			ContentItem parsedItem = parser.Parse("/iTeM2/ItEm2_1.AsPx");
			Assert.AreEqual(item2_1, parsedItem);
		}

		[Test]
		public void CanParseItemWithHash()
		{
			CreateItems(true);

			ContentItem parsedItem = parser.Parse("/item1.aspx#someHash");
			Assert.AreEqual(item1, parsedItem);
		}
		#endregion

		#region Parse Data Tests
		[Test]
		public void CanParseDataItemOnStartPage()
		{
			CreateItems(true);

			ContentItem parsedItem = parser.Parse("/?item=6");
			Assert.AreEqual(data1, parsedItem);
		}

		[Test]
		public void CanParseDataItemOneLevelDown()
		{
			CreateItems(true);

			ContentItem parsedItem = parser.Parse("/item2.aspx?item=7");
			Assert.AreEqual(data2, parsedItem);
		}

		[Test]
		public void CanParseDataItemTwoLevelsDown()
		{
			CreateItems(true);

			ContentItem parsedItem = parser.Parse("/item2/item2_1.aspx?item=8");
			Assert.AreEqual(data3, parsedItem);
		}

		[Test]
		public void ParseNonExistantDataItemReturnsPage()
		{
			CreateItems(false);
			Expect.On(persister).Call(persister.Get(32)).Return(null);
			mocks.ReplayAll();

			ContentItem parsedItem = parser.Parse("/item2/item2_1.aspx?item=32");
			Assert.AreEqual(item2_1, parsedItem);
		}

		[Test]
		public void CanParseDataItemWithMixedCase()
		{
			CreateItems(true);

			ContentItem parsedItem = parser.Parse("/?iTeM=6");
			Assert.AreEqual(data1, parsedItem);
		}


		#endregion

		#region Page BuildUrl Tests
		[Test]
		public void CanCreateStartItemUrl()
		{
			CreateItems(true);

			string url = parser.BuildUrl(startItem);
			Assert.AreEqual("/", url);
		}

		[Test]
		public void CanCreateItemOneLevelDownUrl()
		{
			CreateItems(true);

			string url = parser.BuildUrl(item1);
			Assert.AreEqual("/item1.aspx", url);
		}

		[Test]
		public void CanCreateItemOneStepTwoLevelsDownUrl()
		{
			CreateItems(true);

			string url = parser.BuildUrl(item2_1);
			Assert.AreEqual("/item2/item2_1.aspx", url);
		} 

		#endregion

		#region Data BuildUrl Tests
		[Test]
		public void CanCreateDataItemUrlOnStartPage()
		{
			CreateItems(true);
			string url = parser.BuildUrl(data1);
			Assert.AreEqual("/?item=6", url);
		}

		[Test]
		public void CanCreateDataItemUrlOnPageOneLevelDown()
		{
			CreateItems(true);

			string url = parser.BuildUrl(data2);
			Assert.AreEqual("/item2.aspx?item=7", url);
		}

		[Test]
		public void CanCreateDataItemUrlOnPageTwoLevelsDown()
		{
			CreateItems(true);

			string url = parser.BuildUrl(data3);
			Assert.AreEqual("/item2/item2_1.aspx?item=8", url);
		}

		#endregion

		#region GetPathAndQuery Tests
		[Test]
		public void CanFindRootUrl()
		{
			mocks.ReplayAll();

			string url = parser.GetPathAndQuery("http://www.n2cms.com/");
			Assert.AreEqual("/", url);
		}

		[Test]
		public void CanFindSimpleUrl()
		{
			mocks.ReplayAll();

			string url = parser.GetPathAndQuery("http://www.n2cms.com/item1.aspx");
			Assert.AreEqual("/item1.aspx", url);
		}

		[Test]
		public void CanFindUrlWithQueryString()
		{
			mocks.ReplayAll();

			string url = parser.GetPathAndQuery("http://www.n2cms.com/item1.aspx?item=1");
			Assert.AreEqual("/item1.aspx?item=1", url);
		}

		[Test]
		public void CanFindDeepUrlWithQueryStrings()
		{
			mocks.ReplayAll();

			string url = parser.GetPathAndQuery("http://www.n2cms.com/item1/item2/item3/item4.aspx?item=1&page=2&optional=yes");
			Assert.AreEqual("/item1/item2/item3/item4.aspx?item=1&page=2&optional=yes", url);
		}

		[Test]
		public void CanFindUrlWithHash()
		{
			mocks.ReplayAll();

			string url = parser.GetPathAndQuery("http://www.n2cms.com/item1.aspx#theHash");
			Assert.AreEqual("/item1.aspx#theHash", url);
		}

		#endregion

		//TODO
		//[RowTest]
		//[Row("http://www.n2cms.com/")]
		//[Row("http://n2.libardo.com/")]
		//public void ParsingHostUrlYeldsNull(string url)
		//{
		//    using (mocks.Record())
		//    {
		//        startItem = CreateOneItem<PageItem>(1, "root", null);
		//        Expect.On(wrapper).Call(wrapper.ToAppRelative("/")).Return("/");
		//        mocks.ReplayAll();
		//    }
		//    ContentItem item = parser.Parse(url);
		//    Assert.IsNull(item, "Parsed item wasn't null");
		//}
	}
}
