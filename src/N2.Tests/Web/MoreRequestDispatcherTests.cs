using System.Configuration;
using N2.Configuration;
using N2.Engine;
using N2.Tests.Fakes;
using N2.Tests.Web.Items;
using NUnit.Framework;
using N2.Persistence;
using N2.Web;
using N2.Engine.MediumTrust;

namespace N2.Tests.Web
{
	[TestFixture]
	public class MoreRequestDispatcherTests : ItemPersistenceMockingBase
	{
		FakeRequestLifeCycleHandler handler;
		IUrlParser parser;
		RequestPathProvider dispatcher;
		FakeWebContextWrapper webContext;
		ContentItem root, two, three;
		CustomExtensionItem one;
		ErrorHandler errorHandler;
		IEngine engine;
		ContentAdapterProvider adapterProvider;

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			root = CreateOneItem<PageItem>(1, "root", null);
			one = CreateOneItem<CustomExtensionItem>(2, "one", root);
			two = CreateOneItem<PageItem>(3, "two", one);
			CreateOneItem<DataItem>(4, "four", root);
			three = CreateOneItem<PageItem>(5, "three.3", root);

			webContext = new FakeWebContextWrapper();
			HostSection hostSection = new HostSection();
			parser = new UrlParser(persister, webContext, mocks.Stub<IItemNotifier>(), new Host(webContext, root.ID, root.ID), hostSection);
			errorHandler = new ErrorHandler(webContext, null, null);
			engine = new FakeEngine();
			engine.AddComponentInstance(null, typeof(IWebContext), webContext);
			adapterProvider = new ContentAdapterProvider(engine, new AppDomainTypeFinder());
			adapterProvider.Start();

			ReCreateDispatcherWithConfig(hostSection);
		}



		[Test]
		public void CanRewriteUrl()
		{
			webContext.Url = "/one/two.aspx";

			handler.BeginRequest();

			Assert.That(webContext.rewrittenPath, Is.EqualTo("/default.aspx?page=3"));
		}

		[Test]
		public void RewriteUrl_AppendsExistingQueryString()
		{
			webContext.Url = "/one/two.aspx?happy=true&flip=feet";

			handler.BeginRequest();

			Assert.That(webContext.rewrittenPath, Is.EqualTo("/default.aspx?happy=true&flip=feet&page=3"));
		}

		[Test]
		public void UpdateContentPage()
		{
			webContext.Url = "/one/two.aspx";

			handler.BeginRequest();

			Assert.That(webContext.CurrentPath, Is.Not.Null);
			Assert.That(webContext.CurrentPage, Is.EqualTo(two));
        }

        [Test]
        public void UpdatesCurrentPage_WhenExtension_IsAspx()
        {
			webContext.Url = "/one.aspx";

			handler.BeginRequest();

			Assert.That(webContext.CurrentPage, Is.EqualTo(one));
        }

        [Test]
        public void UpdatesCurrentPage_WhenExtension_IsConfiguredAsObserved()
        {
        	one.extension = ".htm";
			ReCreateDispatcherWithConfig(new HostSection { Web = new WebElement { ObservedExtensions = new CommaDelimitedStringCollection { ".html", ".htm" } } });
        	webContext.Url = "/one.htm";

			handler.BeginRequest();

			Assert.That(webContext.CurrentPage, Is.EqualTo(one));
        }

		[Test]
		public void DoesntUpdateCurrentPage_WhenExtension_IsNotObserved()
		{
			ReCreateDispatcherWithConfig(new HostSection {Web = new WebElement {ObservedExtensions = new CommaDelimitedStringCollection()}});
			webContext.Url = "/one.html";

			handler.BeginRequest();

			Assert.That(webContext.CurrentPage, Is.Null);
		}

		[Test]
		public void UpdatesCurrentPage_WhenAllExtensions_AreObserved_AndOddExtension_IsPassed()
		{
			ReCreateDispatcherWithConfig(new HostSection { Web = new WebElement { ObserveAllExtensions = true} });
			webContext.Url = "/three.3";

			handler.BeginRequest();

			Assert.That(webContext.CurrentPage, Is.EqualTo(three));
		}

		[Test]
		public void DoesntUpdateCurrentPage_WhenExtension_IsEmpty_AndEmpty_IsNotObserved()
		{
			ReCreateDispatcherWithConfig(new HostSection {Web = new WebElement {ObserveEmptyExtension = false}});
			webContext.Url = "/one";

			handler.BeginRequest();

			Assert.That(webContext.CurrentPage, Is.Null);
		}

        [Test]
        public void UpdatesCurrentPage_WhenEmptyExtension_IsConfiguredAsObserved()
        {
        	ReCreateDispatcherWithConfig(new HostSection {Web = new WebElement {ObserveEmptyExtension = true, ObservedExtensions = new CommaDelimitedStringCollection()}});
			webContext.Url = "/one";

			handler.BeginRequest();

			Assert.That(webContext.CurrentPage, Is.EqualTo(one));
        }

		[Test]
		public void UpdateContentPage_WithRewrittenUrl()
		{
			webContext.Url = "/default.aspx?page=3";

			handler.BeginRequest();

			Assert.That(webContext.CurrentPage, Is.EqualTo(two));
		}

		[Test]
		public void UpdateContentPage_WithItemReference_UpdatesWithPage()
		{
			webContext.Url = "/default.aspx?item=4&page=3";

			handler.BeginRequest();

			Assert.That(webContext.CurrentPage, Is.EqualTo(two));
		}

		[Test]
		public void UpdatesCurrentPage_WhenUrl_IsWebDevStartPage()
		{
			webContext.Url = "/default.aspx?";

			handler.BeginRequest();

			Assert.That(webContext.CurrentPage, Is.EqualTo(root));
		}

		void ReCreateDispatcherWithConfig(HostSection config)
		{
			dispatcher = new RequestPathProvider(adapterProvider, webContext, parser, errorHandler, config);
			handler = new FakeRequestLifeCycleHandler(webContext, null, dispatcher, adapterProvider, errorHandler, new EditSection(), new HostSection());
		}
	}
}
