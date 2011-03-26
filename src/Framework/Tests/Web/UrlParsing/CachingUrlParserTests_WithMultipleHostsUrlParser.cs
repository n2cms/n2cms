using N2.Configuration;
using N2.Tests.Fakes;
using N2.Web;
using NUnit.Framework;

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
			parser = new CachingUrlParserDecorator(inner, persister);
			CreateDefaultStructure();
		}

		protected virtual UrlParser GetInnerUrlParser()
		{
			sites = new[] { host.DefaultSite,
			                new Site(1, 2/*item1.ID*/, "www.n2cms.com"), 
			                new Site(1, 4/*item2.ID*/, "n2.libardo.com"), 
			                new Site(1, 5/*item2_1.ID*/, "www.n2cms.com:8080") };

			new MultipleSitesInitializer(persister, host, new StaticSitesProvider(sites), new HostSection() { MultipleSites = true, DynamicSites = true }, null).Start();
			return new MultipleSitesParser(persister, wrapper, host, new HostSection() { MultipleSites = true, DynamicSites = true });
		}

		[Test]
		public void DoesntCacheSameStartPage_ForMultipleDomains()
		{
			var data1 = parser.ResolvePath("http://www.n2cms.com/");
			Assert.That(data1.CurrentItem, Is.EqualTo(repository.Get(2)));
			
			var data2 = parser.ResolvePath("http://n2.libardo.com/");
			Assert.That(data2.CurrentItem, Is.EqualTo(repository.Get(4)));

		}
	}
}