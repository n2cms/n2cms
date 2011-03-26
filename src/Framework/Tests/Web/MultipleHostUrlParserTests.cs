using N2.Tests.Fakes;
using N2.Web;
using N2.Configuration;

namespace N2.Tests.Web
{
	public abstract class MultipleHostUrlParserTests : ParserTestsBase
	{
		protected Site[] sites;
		protected override  UrlParser CreateUrlParser()
		{
			sites = new Site[] { 
                host.DefaultSite,
				new Site(1, 2/*item1.ID*/, "www.n2cms.com"), 
				new Site(1, 4/*item2.ID*/, "n2.libardo.com"), 
				new Site(1, 5/*item2_1.ID*/, "www.n2cms.com:8080") 
			};
			new MultipleSitesInitializer(persister, host, new StaticSitesProvider(sites), new HostSection() { MultipleSites = true, DynamicSites = true }, null).Start();
			MultipleSitesParser parser = new MultipleSitesParser(persister, wrapper, host, new HostSection() { MultipleSites = true, DynamicSites = true });
			return parser;
		}

		protected MultipleSitesParser MultipleParser
		{
			get { return parser as MultipleSitesParser; }
		}
	}
}
