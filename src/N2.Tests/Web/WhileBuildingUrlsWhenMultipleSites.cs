using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace N2.Tests.Web
{
    [TestFixture]
    public class WhileBuildingUrlsWhenMultipleSites : MultipleHostUrlParserTests
    {
        [Test]
        public void CanCreate_Current_StartItem_Url()
        {
            CreateDefaultStructure();
            mocks.ReplayAll();

            string url = parser.BuildUrl(item1);
            Assert.AreEqual("/", url);
        }

        [Test]
        public void CanBuildUrlOnCurrentSite()
        {
            CreateDefaultStructure();
			wrapper.Url = "http://www.n2cms.com/";
            mocks.ReplayAll();

            string url = parser.BuildUrl(item1_1);
            Assert.AreEqual("/item1_1.aspx", url);
        }

        [Test]
        public void CanBuildUrlOnOtherSiteStartPage()
        {
            CreateDefaultStructure();
			wrapper.Url = "http://www.n2cms.com&";
            mocks.ReplayAll();

            string url = parser.BuildUrl(item2);
            Assert.AreEqual("http://n2.libardo.com/", url);
        }

        [Test]
        public void CanBuildUrlOnOtherSitePage()
        {
            CreateDefaultStructure();
			wrapper.Url = "http://n2.libardo.com&";
            mocks.ReplayAll();

            string url = parser.BuildUrl(item1_1);
            Assert.AreEqual("http://www.n2cms.com/item1_1.aspx", url);
        }

        [Test]
        public void ReferencesItems_OutsideAllSites_ByRewrittenUrl()
        {
			wrapper.Url = "http://www.n2cms.com/";
            ContentItem itemOnTheOutside = CreateOneItem<PageItem>(99, "item4", startItem);

            mocks.ReplayAll();

            string url = parser.BuildUrl(itemOnTheOutside);
            Assert.That(url, Is.EqualTo("/default.aspx?page=99"));
        }

        [Test]
        public void DoesntAdd_DefaultSiteTwice()
        {
            int count = (from s in host.Sites where string.IsNullOrEmpty(s.Authority) select 1).Count();

            Assert.That(count, Is.EqualTo(1));
        }
    }
}
