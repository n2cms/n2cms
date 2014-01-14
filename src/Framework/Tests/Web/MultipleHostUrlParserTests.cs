using N2.Configuration;
using N2.Tests.Fakes;
using N2.Web;

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
            var connections = new N2.Plugin.ConnectionMonitor().SetConnected(N2.Edit.Installation.SystemStatusLevel.UpAndRunning);
            new MultipleSitesInitializer(persister, host, new StaticSitesProvider(sites), connections, new HostSection() { MultipleSites = true, DynamicSites = true }, null).Start();
            MultipleSitesParser parser = new MultipleSitesParser(persister, wrapper, host, connections, new HostSection() { MultipleSites = true, DynamicSites = true });
            return parser;
        }

        protected MultipleSitesParser MultipleParser
        {
            get { return parser as MultipleSitesParser; }
        }
    }
}
