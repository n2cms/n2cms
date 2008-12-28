using N2.Engine;
using N2.Tests.Fakes;
using N2.Tests.Web.Items;
using NUnit.Framework;
using N2.Persistence;
using N2.Web;
using NUnit.Framework.SyntaxHelpers;

namespace N2.Tests.Web
{
	[TestFixture]
	public class UrlRewriterTests : ItemPersistenceMockingBase
	{
		FakeRequestLifeCycleHandler handler;
		IUrlParser parser;
		RequestDispatcher rewriter;
		FakeWebContextWrapper context;
		ContentItem root, one, two;

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			root = CreateOneItem<PageItem>(1, "root", null);
			one = CreateOneItem<PageItem>(2, "one", root);
			two = CreateOneItem<PageItem>(3, "two", one);
			CreateOneItem<DataItem>(4, "four", root);

			context = new FakeWebContextWrapper();
			parser = new UrlParser(persister, context, mocks.Stub<IItemNotifier>(), new Host(context, root.ID, root.ID));
			rewriter = new RequestDispatcher(parser, context, new AppDomainTypeFinder());
			handler = new FakeRequestLifeCycleHandler(null, context, null, null, rewriter);
		}


		[Test]
		public void CanRewriteUrl()
		{
			context.Url = new Url("/one/two.aspx");

			handler.BeginRequest();

			Assert.That(context.rewrittenPath, Is.EqualTo("/default.aspx?page=3"));
		}

		[Test]
		public void RewriteUrl_AppendsExistingQueryString()
		{
			context.Url = "/one/two.aspx?happy=true&flip=feet";

			handler.BeginRequest();

			Assert.That(context.rewrittenPath, Is.EqualTo("/default.aspx?happy=true&flip=feet&page=3"));
		}

		[Test]
		public void UpdateContentPage()
		{
			context.Url = "/one/two.aspx";

			handler.BeginRequest();

			Assert.That(context.CurrentPath, Is.Not.Null);
			Assert.That(context.CurrentPage, Is.EqualTo(two));
        }

        [Test]
        public void UpdatesCurrentPage_WhenExtension_IsAspx()
        {
			context.Url = "/one.aspx";

			handler.BeginRequest();

			Assert.That(context.CurrentPage, Is.EqualTo(one));
        }

        [Test]
        public void UpdatesCurrentPage_WhenExtension_IsConfiguredAsObserved()
        {
			//HostSection config = new HostSection { Web = new WebElement { ObservedExtensions = new CommaDelimitedStringCollection { ".html", ".htm" } } };
			//UrlRewriter rewriter = new UrlRewriter(parser, context, null, config);
			context.Url = "/one.htm";

			handler.BeginRequest();

			Assert.That(context.CurrentPage, Is.EqualTo(one));
        }

		//[Test]
		//public void DoesntUpdateCurrentPage_WhenExtension_IsNotObserved()
		//{
		//    //HostSection config = new HostSection { Web = new WebElement { ObservedExtensions = new CommaDelimitedStringCollection()} };
		//    //UrlRewriter rewriter = new UrlRewriter(parser, context, null, config);
		//    context.Url = "/one.html";

		//    handler.BeginRequest();

		//    Assert.That(context.CurrentPage, Is.Null);
		//}

		//[Test]
		//public void DoesntUpdateCurrentPage_WhenExtension_IsEmpty_AndEmpty_IsNotObserved()
		//{
		//    //HostSection config = new HostSection { Web = new WebElement { ObserveEmptyExtension = false } };
		//    //UrlRewriter rewriter = new UrlRewriter(parser, context, null, config);
		//    context.Url = "/one";

		//    handler.BeginRequest();

		//    Assert.That(context.CurrentPage, Is.Null);
		//}

        [Test]
        public void UpdatesCurrentPage_WhenEmptyExtension_IsConfiguredAsObserved()
        {
			//HostSection config = new HostSection { Web = new WebElement { ObserveEmptyExtension = true, ObservedExtensions = new CommaDelimitedStringCollection() } };
			//UrlRewriter rewriter = new UrlRewriter(parser, context, null, config);
			context.Url = "/one";

			handler.BeginRequest();

			Assert.That(context.CurrentPage, Is.EqualTo(one));
        }

		[Test]
		public void UpdateContentPage_WithRewrittenUrl()
		{
			context.Url = "/default.aspx?page=3";

			handler.BeginRequest();

			Assert.That(context.CurrentPage, Is.EqualTo(two));
		}

		[Test]
		public void UpdateContentPage_WithItemReference_UpdatesWithPage()
		{
			context.Url = "/default.aspx?item=4&page=3";

			handler.BeginRequest();

			Assert.That(context.CurrentPage, Is.EqualTo(two));
		}

		[Test]
		public void UpdatesCurrentPage_WhenUrl_IsWebDevStartPage()
		{
			context.Url = "/default.aspx?";

			handler.BeginRequest();

			Assert.That(context.CurrentPage, Is.EqualTo(root));
		}
	}
}
