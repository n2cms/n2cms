using N2.Tests.Fakes;
using NUnit.Framework;
using N2.Persistence;
using N2.Web;
using System.Collections.Specialized;
using N2.Configuration;
using System.Configuration;
using NUnit.Framework.SyntaxHelpers;

namespace N2.Tests.Web
{
	[TestFixture]
	public class UrlRewriterTests : ItemPersistenceMockingBase
	{
		IUrlParser parser;
		UrlRewriter rewriter;
		FakeWebContextWrapper context;
		ContentItem root, one, two;

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			root = CreateOneItem<PageItem>(1, "root", null);
			one = CreateOneItem<PageItem>(2, "one", root);
			two = CreateOneItem<PageItem>(3, "two", one);

			context = new FakeWebContextWrapper();
			parser = new UrlParser(persister, context, mocks.Stub<IItemNotifier>(), new Host(context, root.ID, root.ID));
			rewriter = new UrlRewriter(parser, context);
		}


		[Test]
		public void CanRewriteUrl()
		{
			context.LocalUrl = new Url("/one/two.aspx");

			rewriter.InitializeRequest();
			rewriter.RewriteRequest();

			Assert.That(context.rewrittenPath, Is.EqualTo("/default.aspx?page=3"));
		}

		[Test]
		public void RewriteUrl_AppendsExistingQueryString()
		{
			context.LocalUrl = "/one/two.aspx?happy=true&flip=feet";

			rewriter.InitializeRequest();
			rewriter.RewriteRequest();

			Assert.That(context.rewrittenPath, Is.EqualTo("/default.aspx?happy=true&flip=feet&page=3"));
		}

		[Test]
		public void UpdateContentPage()
		{
			context.LocalUrl = "/one/two.aspx";

			rewriter.InitializeRequest();

			Assert.That(context.CurrentTemplate, Is.Not.Null);
			Assert.That(context.CurrentPage, Is.EqualTo(two));
        }

        [Test]
        public void UpdatesCurrentPage_WhenExtension_IsAspx()
        {
			context.LocalUrl = "/one.aspx";

            rewriter.InitializeRequest();

			Assert.That(context.CurrentPage, Is.EqualTo(one));
        }

        [Test]
        public void UpdatesCurrentPage_WhenExtension_IsConfiguredAsObserved()
        {
            HostSection config = new HostSection { Web = new WebElement { ObservedExtensions = new CommaDelimitedStringCollection { ".html", ".htm" } } };
			UrlRewriter rewriter = new UrlRewriter(parser, context, config);
			context.LocalUrl = "/one.htm";
			
			rewriter.InitializeRequest();

			Assert.That(context.CurrentPage, Is.EqualTo(one));
        }

        [Test]
        public void DoesntUpdateCurrentPage_WhenExtension_IsNotObserved()
        {
            HostSection config = new HostSection { Web = new WebElement { ObservedExtensions = new CommaDelimitedStringCollection()} };
			UrlRewriter rewriter = new UrlRewriter(parser, context, config);
			context.LocalUrl = "/one.html";			
			
			rewriter.InitializeRequest();

			Assert.That(context.CurrentPage, Is.Null);
        }

        [Test]
        public void DoesntUpdateCurrentPage_WhenExtension_IsEmpty_AndEmpty_IsNotObserved()
        {
			HostSection config = new HostSection { Web = new WebElement { ObserveEmptyExtension = false } };
            UrlRewriter rewriter = new UrlRewriter(parser, context, config);
			context.LocalUrl = "/one";

            rewriter.InitializeRequest();

			Assert.That(context.CurrentPage, Is.Null);
        }

        [Test]
        public void UpdatesCurrentPage_WhenEmptyExtension_IsConfiguredAsObserved()
        {
			HostSection config = new HostSection { Web = new WebElement { ObserveEmptyExtension = true, ObservedExtensions = new CommaDelimitedStringCollection() } };
            UrlRewriter rewriter = new UrlRewriter(parser, context, config);
			context.LocalUrl = "/one";

            rewriter.InitializeRequest();

			Assert.That(context.CurrentPage, Is.EqualTo(one));
        }

		[Test]
		public void UpdateContentPage_WithRewrittenUrl()
		{
			context.LocalUrl = "/default.aspx?page=3";
			
			rewriter.InitializeRequest();

			Assert.That(context.CurrentPage, Is.EqualTo(two));
		}

		[Test]
		public void UpdateContentPage_WithItemReference_UpdatesWithPage()
		{
			context.LocalUrl = "/default.aspx?item=2&page=3";

			rewriter.InitializeRequest();

			Assert.That(context.CurrentPage, Is.EqualTo(two));
		}
	}
}
