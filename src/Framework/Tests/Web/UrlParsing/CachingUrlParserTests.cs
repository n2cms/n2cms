using N2.Configuration;
using N2.Tests.Fakes;
using N2.Web;
using NUnit.Framework;
using System.Web;
using System.Collections;

namespace N2.Tests.Web.UrlParsing
{
	[TestFixture]
	public class CachingUrlParserTests : ParserTestsBase
	{
		new protected FakeRepository<ContentItem> repository;

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();
			UrlParser inner = new UrlParser(persister, wrapper, host, new HostSection());
			parser = new CachingUrlParserDecorator(inner, persister);
			CreateDefaultStructure();
			repository = (FakeRepository<ContentItem>) persister.Repository;
		}

		[TearDown]
		public override void TearDown()
		{
			foreach (DictionaryEntry entry in HttpRuntime.Cache)
			{
				HttpRuntime.Cache.Remove(entry.Key.ToString());
			}
			base.TearDown();
		}

		#region Forward to decorated tests
		[Test]
		public void WillForward_BuildUrl()
		{
			var url = parser.BuildUrl(item1);

			Assert.That(url, Is.EqualTo("/item1.aspx"));
		}

		[Test]
		public void WillForward_CurrentPage()
		{
			wrapper.CurrentPage = item1_1;
			var currentPage = parser.CurrentPage;

			Assert.That(currentPage, Is.EqualTo(item1_1));
		}

		[Test]
		public void WillForward_IsRootOrStartPage()
		{
			var isRootOrStartPage = parser.IsRootOrStartPage(startItem);

			Assert.That(isRootOrStartPage, Is.True);
		}

		[Test]
		public void WillForward_Parse()
		{
			var page = parser.Parse("/item1");

			Assert.That(page, Is.EqualTo(item1));
		}

		[Test]
		public void WillForward_ResolveTemplate()
		{
			var data = parser.ResolvePath("/item1");

			Assert.That(data.CurrentItem, Is.EqualTo(item1));
		}

		[Test]
		public void WillForward_StartPage()
		{
			var page = parser.StartPage;

			Assert.That(page, Is.EqualTo(startItem));
		}
		#endregion

		[Test]
		public void CanCache_ResolvedTemplate()
		{
			parser.ResolvePath("/item1/item1_1"); // find and cache

			var data = parser.ResolvePath("/item1/item1_1");

			Assert.That(repository.lastOperation, Is.EqualTo("Load(3)"), "Should have loaded the parsed item directly.");
			Assert.That(data.CurrentItem, Is.EqualTo(item1_1));
		}

		[Test]
		public void WillExpire_ResolvedTemplate_OnChanges()
		{
			parser.ResolvePath("/item1/item1_1"); // find and cache
			persister.Save(startItem); // incur changes

			var data = parser.ResolvePath("/item1/item1_1");

			Assert.That(repository.lastOperation, Is.EqualTo("Load(1)"), "Should have re-resolve template from start page.");
			Assert.That(data.CurrentItem, Is.EqualTo(item1_1));
		}
	}
}
