using System;
using System.Collections.Generic;
using NUnit.Framework;
using Rhino.Mocks;
using N2.Persistence;
using N2.Web;

namespace N2.Tests.Web
{
	[TestFixture]
	public class MultipleHostUrlParserTests : ParserTestsBase
	{
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

			parser = CreateUrlParser();
		}

		protected override IWebContext CreateWrapper(bool replay)
		{
			return base.CreateWrapper(false);
		}

		Site[] sites;
		protected MultipleHostsUrlParser CreateUrlParser()
		{
			sites = new Site[] { 
				host.DefaultSite,
				new Site(1, 2/*item1.ID*/, "www.n2cms.com"), 
				new Site(1, 4/*item2.ID*/, "n2.libardo.com"), 
				new Site(1, 5/*item2_1.ID*/, "www.n2cms.com:8080") 
			};
			MultipleHostsUrlParser parser = new MultipleHostsUrlParser(persister, wrapper, notifier, host, new StaticSitesProvider(sites));
			return parser;
		}

		protected MultipleHostsUrlParser MultipleParser
		{
			get { return parser as MultipleHostsUrlParser; }
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
			Expect.On(wrapper).Call(wrapper.Host).Return("www.n2cms.com");
			mocks.ReplayAll();

			Assert.AreSame(sites[1], parser.CurrentSite);
		}

		[Test]
		public void CanFindSiteWithPort()
		{
			Expect.On(wrapper).Call(wrapper.Host).Return("www.n2cms.com:8080");
			mocks.ReplayAll();

			Assert.AreSame(sites[3], parser.CurrentSite);
		}

		[Test]
		public void FallbacksToDefaultSite()
		{
			Expect.On(wrapper).Call(wrapper.Host).Return("www.siteX.com");
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
			CreateItems(true);
			Expect.On(wrapper).Call(wrapper.Host).Return(host).Repeat.Any();
			Expect.On(wrapper).Call(wrapper.ToAppRelative(url)).Return(url).Repeat.Any();
			mocks.ReplayAll();
		}
		#endregion

		#region Build Url Tests
		[Test]
		public void CanCreateCurrentStartItemUrl()
		{
			CreateItems(true);
			//Expect.On(wrapper).Call(wrapper.ToAbsolute("~/")).Return("/");
			Expect.On(wrapper).Call(wrapper.Host).Return("www.n2cms.com");
			mocks.ReplayAll();

			string url = parser.BuildUrl(item1);
			Assert.AreEqual("/", url);
		}

		[Test]
		public void CanBuildUrlOnCurrentSite()
		{
			CreateItems(true);
			//Expect.On(wrapper).Call(wrapper.ToAbsolute("~/item1_1.aspx")).Return("/item1_1.aspx");
			Expect.On(wrapper).Call(wrapper.Host).Return("www.n2cms.com").Repeat.Any();
			mocks.ReplayAll();

			string url = parser.BuildUrl(item1_1);
			Assert.AreEqual("/item1_1.aspx", url);
		}

		[Test]
		public void CanBuildUrlOnOtherSiteStartPage()
		{
			CreateItems(true);
			//Expect.On(wrapper).Call(wrapper.ToAbsolute("~/")).Return("/");
			Expect.On(wrapper).Call(wrapper.Host).Return("www.n2cms.com").Repeat.Any();
			mocks.ReplayAll();

			string url = parser.BuildUrl(item2);
			Assert.AreEqual("http://n2.libardo.com/", url);
		}

		[Test]
		public void CanBuildUrlOnOtherSitePage()
		{
			CreateItems(true);
			Expect.On(wrapper).Call(wrapper.ToAbsolute("~/item1_1.aspx")).Return("/item1_1.aspx").Repeat.Any();
			Expect.On(wrapper).Call(wrapper.Host).Return("n2.libardo.com").Repeat.Any();
			mocks.ReplayAll();

			string url = parser.BuildUrl(item1_1);
			Assert.AreEqual("http://www.n2cms.com/item1_1.aspx", url);
		}
		#endregion
	}
}
