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
	[TestFixture]
	public class MultipleHostUrlParserTests : ParserTestsBase
	{
        MultipleSitesParser multipleParser;

		class StaticSitesProvider : ISitesProvider
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

		#region SetUp

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

		Site[] sites;
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
		#endregion

		#region GetHostName Tests

		[Test]
		public void CanFindSimpleHostName()
		{
			mocks.ReplayAll();

			string host = MultipleParser.GetHost("http://www.n2cms.com/");
			Assert.AreEqual("www.n2cms.com", host);
		}

		[Test]
		public void CanFindHostNameNoTrailingSlash()
		{
			mocks.ReplayAll();
			
			string host = MultipleParser.GetHost("http://www.n2cms.com");
			Assert.AreEqual("www.n2cms.com", host);
		}

		[Test]
		public void CanFindHostNameWithUrl()
		{
			mocks.ReplayAll();

			string host = MultipleParser.GetHost("http://www.n2cms.com/item1.aspx");
			Assert.AreEqual("www.n2cms.com", host);
		}

		[Test]
		public void CanFindHostNameHttps()
		{
			mocks.ReplayAll();

			string host = MultipleParser.GetHost("https://www.n2cms.com/");
			Assert.AreEqual("www.n2cms.com", host);
		}

		[Test]
		public void CanFindHostPort8080()
		{
			mocks.ReplayAll();

			string host = MultipleParser.GetHost("https://www.n2cms.com:8080/");
			Assert.AreEqual("www.n2cms.com:8080", host);
		}
		#endregion

		#region GetSite Tests
		[Test]
		public void CanGetFirstSite()
		{
			mocks.ReplayAll();

			Site s = MultipleParser.GetSite("www.n2cms.com");
			Assert.AreSame(sites[1], s);
		}

		[Test]
		public void CanGetSiteWithPort()
		{
			mocks.ReplayAll();

			Site s = MultipleParser.GetSite("www.n2cms.com:8080");
			Assert.AreSame(sites[3], s);
		}

		#endregion

		#region CurrentSite Tests
		[Test]
		public void CanFindASite()
		{
            currentHost = "www.n2cms.com";
			mocks.ReplayAll();

			Assert.AreSame(sites[1], parser.CurrentSite);
		}

		[Test]
		public void CanFindSiteWithPort()
		{
            currentHost = "www.n2cms.com:8080";
			mocks.ReplayAll();

			Assert.AreSame(sites[3], parser.CurrentSite);
		}

		[Test]
		public void FallbacksToDefaultSite()
		{
            currentHost = "www.siteX.com";
			mocks.ReplayAll();

			Assert.AreSame(host.DefaultSite, parser.CurrentSite);
		}
		#endregion

		#region Parse Start Pages
		[Test]
		public void CanParseStartPageUrl()
		{
            CreateItemsAndBuildExpectations("www.n2cms.com", "/");

			Assert.AreSame(item1, parser.Parse("/"));
		}

		[Test]
		public void CanParseSubPageUrl()
		{
			CreateItemsAndBuildExpectations("www.n2cms.com", "/item1_1.aspx");

			Assert.AreSame(item1_1, parser.Parse("/item1_1.aspx"));
		}

		[Test]
		public void CanParseSiteWithPortStartPage()
		{
			CreateItemsAndBuildExpectations("www.n2cms.com:8080", "/");

			Assert.AreSame(item2_1, parser.Parse("/"));
		}

		[Test]
		public void CanParseStartPageOnOtherSite()
		{
			CreateItemsAndBuildExpectations("www.n2cms.com:8080", "/");

			Assert.AreSame(item1, parser.Parse("http://www.n2cms.com/"));
		}

		[Test]
		public void CanParseSubPageOnOtherSite()
		{
			CreateItemsAndBuildExpectations("www.n2cms.com:8080", "/item1_1.aspx");

			Assert.AreSame(item1_1, parser.Parse("http://www.n2cms.com/item1_1.aspx"));
		}

		[Test]
		public void CanParseDataItem()
		{
			CreateItemsAndBuildExpectations("n2.libardo.com", "/item2_1.aspx?item=7");

			Assert.AreSame(data2, parser.Parse("/item2_1.aspx?item=7"));
		}

		[Test]
		public void ParsePage_WithNonInteger_PageQueryString_ReturnsNull()
		{
			CreateItemsAndBuildExpectations("n2.libardo.com", "http://www.externalsite.com/somepage.html?item=this_isnt_a_number");

			Assert.IsNull(parser.Parse("http://www.externalsite.com/somepage.html?item=this_isnt_a_number"));
		}

		[Test]
		public void ParsePage_WithNonInteger_ItemQueryString_ReturnsNull()
		{
			CreateItemsAndBuildExpectations("n2.libardo.com", "http://www.externalsite.com/somepage.html?item=this_isnt_a_number");

			Assert.IsNull(parser.Parse("http://www.externalsite.com/somepage.html?item=this_isnt_a_number"));
		}

		private void CreateItemsAndBuildExpectations(string host, string url)
		{
			CreateDefaultStructure();
            currentHost = host;
			mocks.ReplayAll();
		}
		#endregion

		#region Build Url Tests
        
		[Test]
		public void CanCreateCurrentStartItemUrl()
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
            currentHost = "www.n2cms.com";
			mocks.ReplayAll();

			string url = parser.BuildUrl(item1_1);
			Assert.AreEqual("/item1_1.aspx", url);
		}

		[Test]
		public void CanBuildUrlOnOtherSiteStartPage()
		{
			CreateDefaultStructure();
            currentHost = "www.n2cms.com";
			mocks.ReplayAll();

			string url = parser.BuildUrl(item2);
			Assert.AreEqual("http://n2.libardo.com/", url);
		}

		[Test]
		public void CanBuildUrlOnOtherSitePage()
		{
			CreateDefaultStructure();
            currentHost = "n2.libardo.com"; 
			mocks.ReplayAll();

			string url = parser.BuildUrl(item1_1);
			Assert.AreEqual("http://www.n2cms.com/item1_1.aspx", url);
		}

        [Test]
        public void ReferencesItems_OutsideAllSites_ByRewrittenUrl()
        {
            currentHost = "www.n2cms.com";
            ContentItem itemOnTheOutside = CreateOneItem<PageItem>(99, "item4", startItem);

            mocks.ReplayAll();

            string url = parser.BuildUrl(itemOnTheOutside);
            Assert.That(url, Is.EqualTo("/default.aspx?page=99"));
        }

        [Test]
        public void DoesntAddDefaultSiteTwice()
        {
            int count = (from s in multipleParser.Sites where string.IsNullOrEmpty(s.Authority) select 1).Count();

            Assert.That(count, Is.EqualTo(1));
        }

		#endregion
	}
}
