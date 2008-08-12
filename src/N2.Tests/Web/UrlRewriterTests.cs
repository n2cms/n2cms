using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using NUnit.Framework;
using N2.Persistence;
using N2.Web;
using Rhino.Mocks;
using System.Collections.Specialized;
using N2.Configuration;
using System.Configuration;

namespace N2.Tests.Web
{
	[TestFixture]
	public class UrlRewriterTests : ItemTestsBase
	{
		IPersister persister;
		IUrlParser parser;

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();
			persister = mocks.Stub<IPersister>();
			parser = mocks.StrictMock<IUrlParser>();
		}

		[Test]
		public void CanRewriteUrl()
		{
			ContentItem root = CreateOneItem<PageItem>(1, "root", null);
			ContentItem one = CreateOneItem<PageItem>(2, "one", root);
			ContentItem two = CreateOneItem<PageItem>(3, "two", one);

			IWebContext context = mocks.StrictMock<IWebContext>();
			Expect.On(context).Call(context.CurrentPage).Return(two).Repeat.Once();
            Expect.Call(context.LocalUrl).Return(new Url("/one/two.aspx")).Repeat.Any();
			Expect.On(context).Call(context.PhysicalPath).Return(AppDomain.CurrentDomain.BaseDirectory + "\\one\\two.aspx").Repeat.Any();
			context.RewritePath("/default.aspx?page=3");
			LastCall.Repeat.Once();
			mocks.ReplayAll();

			UrlRewriter rewriter = new UrlRewriter(parser, context);
			rewriter.RewriteRequest();
		}

		[Test]
		public void RewriteUrl_AppendsExistingQueryString()
		{
			ContentItem root = CreateOneItem<PageItem>(1, "root", null);
			ContentItem one = CreateOneItem<PageItem>(2, "one", root);
			ContentItem two = CreateOneItem<PageItem>(3, "two", one);

			IWebContext context = mocks.StrictMock<IWebContext>();
			Expect.On(context).Call(context.CurrentPage).Return(two).Repeat.Any();
            Expect.Call(context.LocalUrl).Return(new Url("/one/two.aspx?happy=true&flip=feet")).Repeat.Any();
			Expect.On(context).Call(context.PhysicalPath).Return(AppDomain.CurrentDomain.BaseDirectory + "\\one\\two.aspx").Repeat.Any();
            context.RewritePath("/default.aspx?page=3&happy=true&flip=feet");
			LastCall.Repeat.Once();
			mocks.ReplayAll();

			UrlRewriter rewriter = new UrlRewriter(parser, context);
			rewriter.RewriteRequest();
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
			ContentItem root = CreateOneItem<PageItem>(1, "root", null);
			ContentItem one = CreateOneItem<PageItem>(2, "one", root);
			ContentItem two = CreateOneItem<PageItem>(3, "two", one);

			Expect.On(parser).Call(parser.ParsePage("/one/two.aspx")).Return(two);
			
			IWebContext context = mocks.StrictMock<IWebContext>();
            Expect.Call(context.LocalUrl).Return(new Url("/one/two.aspx")).Repeat.Any();
			Expect.Call(context.CurrentPage).Return(null).Repeat.Any();
			Expect.Call(delegate{ context.CurrentPage = two; });
			
			mocks.ReplayAll();

			UrlRewriter rewriter = new UrlRewriter(parser, context);
			rewriter.UpdateCurrentPage();
        }

        [Test]
        public void UpdatesCurrentPage_WhenExtension_IsAspx()
        {
            ContentItem root = CreateOneItem<PageItem>(1, "root", null);
            ContentItem one = CreateOneItem<PageItem>(2, "one", root);
            
            Expect.On(parser).Call(parser.ParsePage("/one.aspx")).Return(one);

            IWebContext context = mocks.StrictMock<IWebContext>();
            Expect.Call(context.LocalUrl).Return(new Url("/one.aspx")).Repeat.Any();
            Expect.Call(context.CurrentPage).Return(null).Repeat.Any();
            Expect.Call(delegate { context.CurrentPage = one; });

            mocks.ReplayAll();

            UrlRewriter rewriter = new UrlRewriter(parser, context);
            rewriter.UpdateCurrentPage();
        }

        [Test]
        public void UpdatesCurrentPage_WhenExtension_IsConfiguredAsObserved()
        {
            ContentItem root = CreateOneItem<PageItem>(1, "root", null);
            ContentItem one = CreateOneItem<PageItem>(2, "one", root);

            Expect.On(parser).Call(parser.ParsePage("/one.htm")).Return(one);

            IWebContext context = mocks.StrictMock<IWebContext>();
            Expect.Call(context.LocalUrl).Return(new Url("/one.htm")).Repeat.Any();
            Expect.Call(context.CurrentPage).Return(null).Repeat.Any();
            Expect.Call(delegate { context.CurrentPage = one; });

            mocks.ReplayAll();

            HostSection config = new HostSection { Web = new WebElement { ObservedExtensions = new CommaDelimitedStringCollection { ".html", ".htm" } } };

            UrlRewriter rewriter = new UrlRewriter(parser, context, config);
            rewriter.UpdateCurrentPage();
        }

        [Test]
        public void DoesntUpdateCurrentPage_WhenExtension_IsHtml()
        {
            ContentItem root = CreateOneItem<PageItem>(1, "root", null);
            ContentItem one = CreateOneItem<PageItem>(2, "one", root);

            Expect.On(parser).Call(parser.ParsePage("/one.html")).Repeat.Never();

            IWebContext context = mocks.StrictMock<IWebContext>();
            Expect.Call(context.LocalUrl).Return(new Url("/one.html")).Repeat.Any();
            Expect.Call(context.CurrentPage).Return(null).Repeat.Any();
            
            mocks.ReplayAll();

            UrlRewriter rewriter = new UrlRewriter(parser, context);
            rewriter.UpdateCurrentPage();
        }

        [Test]
        public void DoesntUpdateCurrentPage_WhenExtension_IsEmpty()
        {
            ContentItem root = CreateOneItem<PageItem>(1, "root", null);
            ContentItem one = CreateOneItem<PageItem>(2, "one", root);

            Expect.On(parser).Call(parser.ParsePage("/one")).Repeat.Never();

            IWebContext context = mocks.StrictMock<IWebContext>();
            Expect.Call(context.LocalUrl).Return(new Url("/one")).Repeat.Any();
            Expect.Call(context.CurrentPage).Return(null).Repeat.Any();

            mocks.ReplayAll();

            UrlRewriter rewriter = new UrlRewriter(parser, context);
            rewriter.UpdateCurrentPage();
        }

        [Test]
        public void UpdatesCurrentPage_WhenEmptyExtension_IsConfiguredAsObserved()
        {
            ContentItem root = CreateOneItem<PageItem>(1, "root", null);
            ContentItem one = CreateOneItem<PageItem>(2, "one", root);

            Expect.On(parser).Call(parser.ParsePage("/one")).Return(one);

            IWebContext context = mocks.StrictMock<IWebContext>();
            Expect.Call(context.LocalUrl).Return(new Url("/one")).Repeat.Any();
            Expect.Call(context.CurrentPage).Return(null).Repeat.Any();
            Expect.Call(delegate { context.CurrentPage = one; });

            mocks.ReplayAll();

            HostSection config = new HostSection { Web = new WebElement { ObserveEmptyExtension = true, ObservedExtensions = new CommaDelimitedStringCollection() } };

            UrlRewriter rewriter = new UrlRewriter(parser, context, config);
            rewriter.UpdateCurrentPage();
        }

		[Test]
		public void UpdateContentPage_WithRewrittenUrl()
		{
			ContentItem root = CreateOneItem<PageItem>(1, "root", null);
			ContentItem one = CreateOneItem<PageItem>(2, "one", root);
			ContentItem two = CreateOneItem<PageItem>(3, "two", one);

			Expect.On(parser).Call(parser.ParsePage("/default.aspx?page=3")).Return(two);

			IWebContext context = mocks.StrictMock<IWebContext>();
            Expect.Call(context.LocalUrl).Return(new Url("/default.aspx?page=3")).Repeat.Any();
            Expect.Call(context.CurrentPage).Return(null).Repeat.Any();
			Expect.Call(delegate { context.CurrentPage = two; });

			mocks.ReplayAll();

			UrlRewriter rewriter = new UrlRewriter(parser, context);
			rewriter.UpdateCurrentPage();
		}

		[Test]
		public void UpdateContentPage_WithItemReference_UpdatesWithPage()
		{
			ContentItem root = CreateOneItem<PageItem>(1, "root", null);
			ContentItem one = CreateOneItem<PageItem>(2, "one", root);
			ContentItem two = CreateOneItem<PageItem>(3, "two", one);

			Expect.On(parser).Call(parser.ParsePage("/default.aspx?page=3&item=2")).Return(two);

			IWebContext context = mocks.StrictMock<IWebContext>();
            Expect.On(context).Call(context.LocalUrl).Return(new Url("/default.aspx?page=3&item=2")).Repeat.Any();
            Expect.Call(context.CurrentPage).Return(null).Repeat.Any();
			Expect.Call(delegate { context.CurrentPage = two; });

			mocks.ReplayAll();

			UrlRewriter rewriter = new UrlRewriter(parser, context);
			rewriter.UpdateCurrentPage();
		}
	}
}
