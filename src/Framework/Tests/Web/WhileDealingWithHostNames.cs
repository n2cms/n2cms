using N2.Web;
using NUnit.Framework;

namespace N2.Tests.Web
{
    [TestFixture]
    public class WhileDealingWithHostNames : MultipleHostUrlParserTests
    {
        [Test]
        public void CanFindSimpleHostName()
        {
            mocks.ReplayAll();

            string host = Url.Parse("http://www.n2cms.com/").Authority;
            Assert.AreEqual("www.n2cms.com", host);
        }

        [Test]
        public void CanFindHostNameNoTrailingSlash()
        {
            mocks.ReplayAll();

            string host = Url.Parse("http://www.n2cms.com").Authority;
            Assert.AreEqual("www.n2cms.com", host);
        }

        [Test]
        public void CanFindHostNameWithUrl()
        {
            mocks.ReplayAll();

            string host = Url.Parse("http://www.n2cms.com/item1.aspx").Authority;
            Assert.AreEqual("www.n2cms.com", host);
        }

        [Test]
        public void CanFindHostNameHttps()
        {
            mocks.ReplayAll();

            string host = Url.Parse("https://www.n2cms.com/").Authority;
            Assert.AreEqual("www.n2cms.com", host);
        }

        [Test]
        public void CanFindHostPort8080()
        {
            mocks.ReplayAll();

            string host = Url.Parse("https://www.n2cms.com:8080/").Authority;
            Assert.AreEqual("www.n2cms.com:8080", host);
        }




        [Test]
        public void CanFind_CurrentSite()
        {
            wrapper.Url = "http://www.n2cms.com/";
            mocks.ReplayAll();

            Assert.AreSame(sites[1], host.CurrentSite);
        }

        [Test]
        public void CanFind_CurrentSite_WithDifferentPort()
        {
            wrapper.Url = "http://www.n2cms.com:8080/";
            mocks.ReplayAll();

            Assert.AreSame(sites[3], host.CurrentSite);
        }

        [Test]
        public void WillFallback_ToDefaultSite_ForUnknownHosts()
        {
            wrapper.Url = "http://www.siteX.com/";
            mocks.ReplayAll();

            Assert.AreSame(host.DefaultSite, host.CurrentSite);
        }
    }
}
