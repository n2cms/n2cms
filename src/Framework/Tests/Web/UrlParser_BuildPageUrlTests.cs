using N2.Configuration;
using N2.Tests.Web.Items;
using N2.Web;
using NUnit.Framework;

namespace N2.Tests.Web
{
    [TestFixture]
    public class UrlParser_BuildPageUrlTests : ParserTestsBase
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
        public void CanBuildUrl_StartItem()
        {
            string url = parser.BuildUrl(startItem);
            Assert.AreEqual("/", url);
        }

        [Test]
        public void CanBuildUrl_ItemOneLevelDown()
        {
            string url = parser.BuildUrl(page1);
            Assert.AreEqual("/item1", url);
        }

        [Test]
        public void CanBuildUrl_ItemOneStep_TwoLevelsDown()
        {
            string url = parser.BuildUrl(page2_1);
            Assert.AreEqual("/item2/item2_1", url);
        }

        [Test]
        public void CanBuildStartPageUrl_InApplication()
        {
            string previousPath = Url.ApplicationPath;
            Url.ApplicationPath = "/HelloWorld/";
            try
            {
                string url = parser.BuildUrl(startItem);

                Assert.AreEqual("/HelloWorld/", url);
            }
            finally
            {
                Url.ApplicationPath = previousPath;
            }
        }

        [Test]
        public void CanBuildSubItemUrl_InApplication()
        {
            string previousPath = Url.ApplicationPath;
            Url.ApplicationPath = "/HelloWorld/";
            try
            {
                string url = parser.BuildUrl(page2_1);

                Assert.AreEqual("/HelloWorld/item2/item2_1", url);
            }
            finally
            {
                Url.ApplicationPath = previousPath;
            }
        }

        [Test]
        public void RevertsToRewrittenUrl_WhenNoParentStartPage()
        {
            PageItem page = CreateOneItem<PageItem>(9, "offsidepage", null);

            string url = parser.BuildUrl(page);

            Assert.AreEqual("/Default.aspx?n2page=9", url);
        }

        [Test]
        public void RevertsToRewrittenUrl_WhenNoParentStartPage_InApplication()
        {
            string previousPath = Url.ApplicationPath;
            Url.ApplicationPath = "/HelloWorld/";
            try
            {
                PageItem page = CreateOneItem<PageItem>(10, "offsidepage", null);

                string url = parser.BuildUrl(page);

                Assert.AreEqual("/HelloWorld/Default.aspx?n2page=10", url);
            }
            finally
            {
                Url.ApplicationPath = previousPath;
            }
        }

        [Test]
        public void AppendsVersionIndex_ToMasterVersion_WhenBuildingUrl_OfVersion()
        {
            PageItem version = CreateOneItem<PageItem>(11, "offsideitem", null);
            version.VersionOf = startItem;
            version.VersionIndex = 22;

            string url = parser.BuildUrl(version);

            Assert.AreEqual("/?n2versionIndex=22", url);
        }
    }
}
