using System;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using Rhino.Mocks;
using N2.Persistence;
using N2.Web;
using N2.Configuration;
using NUnit.Framework.SyntaxHelpers;
using System.Diagnostics;

namespace N2.Tests.Web
{
	public abstract class MultipleHostUrlParserTests : ParserTestsBase
	{
        MultipleSitesParser multipleParser;

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

		
		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

            multipleParser = (MultipleSitesParser)parser;
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
