using N2.Configuration;
using N2.Tests.Web.Items;
using N2.Web;
using NUnit.Framework;
using Shouldly;

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
            base.parser = parser = TestSupport.Setup(persister, wrapper, host);
            CreateDefaultStructure();
        }

        [Test]
        public void CanCreate_DataItemUrl_OnStartPage()
        {
            string url = parser.BuildUrl(part1);
            Assert.AreEqual("/?n2item=6", url);
        }

        [Test]
        public void CanCreate_DataItemUrl_OnPage_OneLevelDown()
        {
            string url = parser.BuildUrl(part2);
            Assert.AreEqual("/item2?n2item=7", url);
        }

        [Test]
        public void CanCreate_DataItemUrl_OnPage_TwoLevelsDown()
        {
            string url = parser.BuildUrl(part3);
            Assert.AreEqual("/item2/item2_1?n2item=8", url);
        }

        [Test]
        public void DataItemUrl_OfVersionedItem__OnStartPage_IsStartPageUrl_AndVersionID()
        {
            DataItem data4 = CreateOneItem<DataItem>(123, "123", null);
            data4.VersionOf = part1;
            data4.VersionIndex = 66;

            string url = parser.BuildUrl(data4);
            url.ShouldBe("/?n2item=6&n2versionIndex=66");
            //Assert.That(url, Is.EqualTo("/?item=123"));
        }

        [Test]
        public void DataItemUrl_OfVersionedItem_IsPageUrl_AndVersionID()
        {
            DataItem data4 = CreateOneItem<DataItem>(123, "123", null);
            data4.VersionOf = part3;
            data4.VersionIndex = 77;

            string url = parser.BuildUrl(data4);
            Assert.That(url, Is.EqualTo("/item2/item2_1?n2item=8&n2versionIndex=77"));
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
