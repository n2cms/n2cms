using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using NUnit.Framework;
using N2.Persistence;
using N2.Web;
using Rhino.Mocks;
using System.Collections.Specialized;

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
