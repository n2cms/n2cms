using N2.Configuration;
using N2.Tests.Web.Items;
using N2.Web;
using NUnit.Framework;

namespace N2.Tests.Web
{
	[TestFixture]
	public class UrlParser_BuildDataUrlTests : ParserTestsBase
	{
		protected new UrlParser parser;

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();
			base.parser = parser = new UrlParser(persister, wrapper, host, new HostSection());
			CreateDefaultStructure();
		}

		[Test]
		public void CanCreate_DataItemUrl_OnStartPage()
		{
			string url = parser.BuildUrl(data1);
			Assert.AreEqual("/?item=6", url);
		}

		[Test]
		public void CanCreate_DataItemUrl_OnPage_OneLevelDown()
		{
			string url = parser.BuildUrl(data2);
			Assert.AreEqual("/item2.aspx?item=7", url);
		}

		[Test]
		public void CanCreate_DataItemUrl_OnPage_TwoLevelsDown()
		{
			string url = parser.BuildUrl(data3);
			Assert.AreEqual("/item2/item2_1.aspx?item=8", url);
		}

		[Test]
		public void DataItemUrl_OfVersionedItem__OnStartPage_IsStartPageUrl_AndVersionID()
		{
			DataItem data4 = CreateOneItem<DataItem>(123, "123", null);
			data4.VersionOf = data1;

			string url = parser.BuildUrl(data4);
			Assert.That(url, Is.EqualTo("/?item=123"));
		}

		[Test]
		public void DataItemUrl_OfVersionedItem_IsPageUrl_AndVersionID()
		{
			DataItem data4 = CreateOneItem<DataItem>(123, "123", null);
			data4.VersionOf = data3;

			string url = parser.BuildUrl(data4);
			Assert.That(url, Is.EqualTo("/item2/item2_1.aspx?item=123"));
		}

		[Test]
		public void ThrowsException_WhenNoParentItem_IsPage()
		{
			DataItem data = CreateOneItem<DataItem>(9, "offsideitem", null);
			ExceptionAssert.Throws<N2Exception>(delegate
				{
					parser.BuildUrl(data);
				});
		}
	}
}