using System.Collections.Generic;
using NUnit.Framework;
using N2.Web;
using N2.Configuration;

namespace N2.Tests.Web
{
	public abstract class MultipleHostUrlParserTests : ParserTestsBase
	{
		protected class StaticSitesProvider : ISitesProvider
		{
			IEnumerable<Site> sites;

			public StaticSitesProvider(IEnumerable<Site> sites)
			{
				this.sites = sites;
			}

			public IEnumerable<Site> GetSites()
			{
				return sites;
			}
		}

		protected override IWebContext CreateWrapper(bool replay)
		{
			return base.CreateWrapper(false);
		}

		protected Site[] sites;
		protected override  UrlParser CreateUrlParser()
		{
			sites = new Site[] { 
                host.DefaultSite,
				new Site(1, 2/*item1.ID*/, "www.n2cms.com"), 
				new Site(1, 4/*item2.ID*/, "n2.libardo.com"), 
				new Site(1, 5/*item2_1.ID*/, "www.n2cms.com:8080") 
			};
			MultipleSitesParser parser = new MultipleSitesParser(persister, wrapper, notifier, host, new StaticSitesProvider(sites), new HostSection() { MultipleSites = true, DynamicSites = true });
			return parser;
		}

		protected MultipleSitesParser MultipleParser
		{
			get { return parser as MultipleSitesParser; }
		}
	}
}
