using System;
using System.Collections.Generic;
using MbUnit.Framework;
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
			ICollection<Site> sites;

			public StaticSitesProvider(ICollection<Site> sites)
			{
				this.sites = sites;
			}

			public ICollection<Site> GetSites()
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
				site,
				new Site(1, 2/*item1.ID*/, "www.n2cms.com"), 
				new Site(1, 4/*item2.ID*/, "n2.libardo.com"), 
				new Site(1, 5/*item2_1.ID*/, "www.n2cms.com:8080") 
			};
			MultipleHostsUrlParser parser = new MultipleHostsUrlParser(persister, wrapper, notifier, site, new StaticSitesProvider(sites));
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
			Expect.On(wrapper).Call(wrapper.CurrentHost).Return("www.n2cms.com");
			mocks.ReplayAll();

			Assert.AreSame(sites[1], parser.CurrentSite);
		}

		[Test]
		public void CanFindSiteWithPort()
		{
			Expect.On(wrapper).Call(wrapper.CurrentHost).Return("www.n2cms.com:8080");
			mocks.ReplayAll();

			Assert.AreSame(sites[3], parser.CurrentSite);
		}

		[Test]
		public void FallbacksToDefaultSite()
		{
			Expect.On(wrapper).Call(wrapper.CurrentHost).Return("www.siteX.com");
			mocks.ReplayAll();

			Assert.AreSame(site, parser.CurrentSite);
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

		private void CreateItemsAndBuildExpectations(string host, string url)
		{
			CreateItems();
			Expect.On(wrapper).Call(wrapper.CurrentHost).Return(host);
			Expect.On(wrapper).Call(wrapper.ToAppRelative(url)).Return(url);
			mocks.ReplayAll();
		}
		#endregion

		#region Build Url Tests
		[Test]
		public void CanCreateCurrentStartItemUrl()
		{
			using (mocks.Record())
			{
				CreateItems();
				Expect.On(wrapper).Call(wrapper.ToAbsolute("~/")).Return("/");
				Expect.On(wrapper).Call(wrapper.CurrentHost).Return("www.n2cms.com");
				mocks.ReplayAll();

				string url = parser.BuildUrl(item1);
				Assert.AreEqual("/", url);
			}
		}

		[Test]
		public void CanBuildUrlOnCurrentSite()
		{
			using (mocks.Record())
			{
				CreateItems();
				Expect.On(wrapper).Call(wrapper.ToAbsolute("~/item1_1.aspx")).Return("/item1_1.aspx");
				Expect.On(wrapper).Call(wrapper.CurrentHost).Return("www.n2cms.com");
				Expect.On(wrapper).Call(wrapper.CurrentHost).Return("www.n2cms.com");
				mocks.ReplayAll();

				string url = parser.BuildUrl(item1_1);
				Assert.AreEqual("/item1_1.aspx", url);
			}
		}

		[Test]
		public void CanBuildUrlOnOtherSiteStartPage()
		{
			using (mocks.Record())
			{
				CreateItems();
				Expect.On(wrapper).Call(wrapper.ToAbsolute("~/")).Return("/");
				Expect.On(wrapper).Call(wrapper.CurrentHost).Return("www.n2cms.com");
				Expect.On(wrapper).Call(wrapper.CurrentHost).Return("www.n2cms.com");
				mocks.ReplayAll();

				string url = parser.BuildUrl(item2);
				Assert.AreEqual("http://n2.libardo.com/", url);
			}
		}

		[Test]
		public void CanBuildUrlOnOtherSitePage()
		{
			using (mocks.Record())
			{
				CreateItems();
				Expect.On(wrapper).Call(wrapper.ToAbsolute("~/item1_1.aspx")).Return("/item1_1.aspx");
				Expect.On(wrapper).Call(wrapper.CurrentHost).Return("n2.libardo.com");
				Expect.On(wrapper).Call(wrapper.CurrentHost).Return("n2.libardo.com");
				mocks.ReplayAll();

				string url = parser.BuildUrl(item1_1);
				Assert.AreEqual("http://www.n2cms.com/item1_1.aspx", url);
			}
		}
		#endregion
	}
}
