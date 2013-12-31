using N2.Configuration;
using N2.Tests.Fakes;
using N2.Web;
using NUnit.Framework;
using Shouldly;

namespace N2.Tests.Web.UrlParsing
{
    [TestFixture]
    public class CachingUrlParserTests_WithMultipleHostsUrlParser : ParserTestsBase
    {
        protected Site[] sites;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            UrlParser inner = GetInnerUrlParser();
            parser = new CachingUrlParserDecorator(inner, persister, wrapper, new CacheWrapper(persister, wrapper, new DatabaseSection()), new HostSection());
            CreateDefaultStructure();
        }

        protected virtual UrlParser GetInnerUrlParser()
        {
            sites = new[] { host.DefaultSite,
                            new Site(1, 2/*item1.ID*/, "www.n2cms.com"), 
                            new Site(1, 4/*item2.ID*/, "n2.libardo.com"), 
                            new Site(1, 5/*item2_1.ID*/, "www.n2cms.com:8080") };

            new MultipleSitesInitializer(persister, host, new StaticSitesProvider(sites), new N2.Plugin.ConnectionMonitor().SetConnected(N2.Edit.Installation.SystemStatusLevel.UpAndRunning), new HostSection() { MultipleSites = true, DynamicSites = true }, null).Start();
            return new MultipleSitesParser(persister, wrapper, host, new N2.Plugin.ConnectionMonitor(), new HostSection() { MultipleSites = true, DynamicSites = true });
        }

        [Test]
        public void DoesntCacheSameStartPage_ForMultipleDomains()
        {
            var data1 = parser.FindPath("http://www.n2cms.com/");
            Assert.That(data1.CurrentItem, Is.EqualTo(repository.Get(2)));
            
            var data2 = parser.FindPath("http://n2.libardo.com/");
            Assert.That(data2.CurrentItem, Is.EqualTo(repository.Get(4)));

        }

        [Test]
        public void BuiltUrl_IsDifferent_ForEach_Site()
        {
            wrapper.Url = new Url("http://www.n2cms.com/");
            var url1 = parser.BuildUrl(page1_1).ToString();
            var url2 = parser.BuildUrl(page2).ToString();

            wrapper.Url = new Url("http://n2.libardo.com/");
            var url1b = parser.BuildUrl(page1_1).ToString();
            var url2b = parser.BuildUrl(page2).ToString();

            url1.ShouldNotBe(url1b);
            url2.ShouldNotBe(url2b);
        }
    }
}
