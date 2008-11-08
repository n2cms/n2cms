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
		FakeWebContextWrapper context;
		ContentItem root;

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			context = new FakeWebContextWrapper();
			root = CreateOneItem<PageItem>(1, "root", null);
			
			parser = new UrlParser(persister, context, mocks.Stub<IItemNotifier>(), new Host(context, root.ID, root.ID));
			mocks.ReplayAll();
		}

		[Test]
		public void CanRewriteUrl()
		{
			ContentItem one = CreateOneItem<PageItem>(2, "one", root);
			ContentItem two = CreateOneItem<PageItem>(3, "two", one);

			context.LocalUrl = new Url("/one/two.aspx");

			UrlRewriter rewriter = new UrlRewriter(parser, context);
			rewriter.UpdateCurrentPage();

			rewriter.RewriteRequest();

			Assert.That(context.rewrittenPath, Is.EqualTo("/default.aspx?page=3"));
		}

		[Test]
		public void RewriteUrl_AppendsExistingQueryString()
		{
			ContentItem one = CreateOneItem<PageItem>(2, "one", root);
			ContentItem two = CreateOneItem<PageItem>(3, "two", one);

			context.LocalUrl = "/one/two.aspx?happy=true&flip=feet";
			UrlRewriter rewriter = new UrlRewriter(parser, context);
			rewriter.UpdateCurrentPage();

			rewriter.RewriteRequest();

			Assert.That(context.rewrittenPath, Is.EqualTo("/default.aspx?page=3&happy=true&flip=feet"));
		}

		private NameValueCollection ToNameValueCollection(string p)
		{
			NameValueCollection qs = new NameValueCollection();
			foreach (string param in p.Split('&'))
			{
				string[] pair = param.Split('=');
				qs[pair[0]] = pair[1];
			}
			return qs;
		}

		[Test]
		public void UpdateContentPage()
		{
			ContentItem one = CreateOneItem<PageItem>(2, "one", root);
			ContentItem two = CreateOneItem<PageItem>(3, "two", one);
			context.LocalUrl = "/one/two.aspx";

			UrlRewriter rewriter = new UrlRewriter(parser, context);

			rewriter.UpdateCurrentPage();

			Assert.That(context.CurrentTemplate, Is.Not.Null);
			Assert.That(context.CurrentPage, Is.EqualTo(two));
        }

        [Test]
        public void UpdatesCurrentPage_WhenExtension_IsAspx()
        {
            ContentItem one = CreateOneItem<PageItem>(2, "one", root);
			context.LocalUrl = "/one.aspx";
            mocks.ReplayAll();

            UrlRewriter rewriter = new UrlRewriter(parser, context);

            rewriter.UpdateCurrentPage();

			Assert.That(context.CurrentPage, Is.EqualTo(one));
        }

        [Test]
        public void UpdatesCurrentPage_WhenExtension_IsConfiguredAsObserved()
        {
            HostSection config = new HostSection { Web = new WebElement { ObservedExtensions = new CommaDelimitedStringCollection { ".html", ".htm" } } };
			ContentItem one = CreateOneItem<PageItem>(2, "one", root);
			context.LocalUrl = "/one.htm";
			
			UrlRewriter rewriter = new UrlRewriter(parser, context, config);
            
			rewriter.UpdateCurrentPage();

			Assert.That(context.CurrentPage, Is.EqualTo(one));
        }

        [Test]
        public void DoesntUpdateCurrentPage_WhenExtension_IsNotObserved()
        {
            HostSection config = new HostSection { Web = new WebElement { ObservedExtensions = new CommaDelimitedStringCollection()} };
			ContentItem one = CreateOneItem<PageItem>(2, "one", root);
			context.LocalUrl = "/one.html";
			
			UrlRewriter rewriter = new UrlRewriter(parser, context, config);
			
			rewriter.UpdateCurrentPage();

			Assert.That(context.CurrentPage, Is.Null);
        }

        [Test]
        public void DoesntUpdateCurrentPage_WhenExtension_IsEmpty_AndEmpty_IsNotObserved()
        {
			HostSection config = new HostSection { Web = new WebElement { ObserveEmptyExtension = false } };
			ContentItem one = CreateOneItem<PageItem>(2, "one", root);
			context.LocalUrl = "/one";

            UrlRewriter rewriter = new UrlRewriter(parser, context, config);

            rewriter.UpdateCurrentPage();

			Assert.That(context.CurrentPage, Is.Null);
        }

        [Test]
        public void UpdatesCurrentPage_WhenEmptyExtension_IsConfiguredAsObserved()
        {
			HostSection config = new HostSection { Web = new WebElement { ObserveEmptyExtension = true, ObservedExtensions = new CommaDelimitedStringCollection() } };
			ContentItem one = CreateOneItem<PageItem>(2, "one", root);
			context.LocalUrl = "/one";

            UrlRewriter rewriter = new UrlRewriter(parser, context, config);
            rewriter.UpdateCurrentPage();

			Assert.That(context.CurrentPage, Is.EqualTo(one));
        }

		[Test]
		public void UpdateContentPage_WithRewrittenUrl()
		{
			ContentItem one = CreateOneItem<PageItem>(2, "one", root);
			ContentItem two = CreateOneItem<PageItem>(3, "two", one);
			context.LocalUrl = "/default.aspx?page=3";

			UrlRewriter rewriter = new UrlRewriter(parser, context);
			
			rewriter.UpdateCurrentPage();

			Assert.That(context.CurrentPage, Is.EqualTo(two));
		}

		[Test]
		public void UpdateContentPage_WithItemReference_UpdatesWithPage()
		{
			ContentItem one = CreateOneItem<PageItem>(2, "one", root);
			ContentItem two = CreateOneItem<PageItem>(3, "two", one);
			context.LocalUrl = "/default.aspx?item=2&page=3";

			UrlRewriter rewriter = new UrlRewriter(parser, context);

			rewriter.UpdateCurrentPage();

			Assert.That(context.CurrentPage, Is.EqualTo(two));
		}
	}
}
