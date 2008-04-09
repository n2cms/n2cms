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
			parser = mocks.CreateMock<IUrlParser>();
			Expect.On(parser).Call(parser.DefaultExtension).Return(".aspx").Repeat.Any();
		}

		[Test]
		public void CanRewriteUrl()
		{
			ContentItem root = CreateOneItem<PageItem>(1, "root", null);
			ContentItem one = CreateOneItem<PageItem>(2, "one", root);
			ContentItem two = CreateOneItem<PageItem>(3, "two", one);

			IWebContext context = mocks.CreateMock<IWebContext>();
			Expect.On(context).Call(context.CurrentPage).Return(two).Repeat.Once();
			Expect.On(context).Call(context.QueryString).Return(new NameValueCollection()).Repeat.Once();
			Expect.On(context).Call(context.AbsolutePath).Return("/one/two.aspx").Repeat.Any();
			Expect.On(context).Call(context.RawUrl).Return("/one/two.aspx").Repeat.Any();
			Expect.On(context).Call(context.PhysicalPath).Return(AppDomain.CurrentDomain.BaseDirectory + "\\one\\two.aspx").Repeat.Any();
			context.RewritePath("/default.aspx?page=3");
			LastCall.Repeat.Once();
			mocks.ReplayAll();

			UrlRewriter rewriter = new UrlRewriter(parser, context);
			rewriter.RewriteRequest();
		}

		[Test]
		public void RewriteUrlAppendsExistingQueryString()
		{
			ContentItem root = CreateOneItem<PageItem>(1, "root", null);
			ContentItem one = CreateOneItem<PageItem>(2, "one", root);
			ContentItem two = CreateOneItem<PageItem>(3, "two", one);

			IWebContext context = mocks.CreateMock<IWebContext>();
			Expect.On(context).Call(context.CurrentPage).Return(two).Repeat.Any();
			Expect.On(context).Call(context.QueryString).Return(ToNameValueCollection("happy=true&flip=feet"));
			Expect.On(context).Call(context.Query).Return("happy=true&flip=feet");
			Expect.On(context).Call(context.AbsolutePath).Return("/one/two.aspx").Repeat.Any();
			Expect.On(context).Call(context.RawUrl).Return("/one/two.aspx").Repeat.Any();
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

			Expect.On(parser).Call(parser.Parse("/one/two.aspx")).Return(two);
			
			IWebContext context = mocks.CreateMock<IWebContext>();
			Expect.On(context).Call(context.AbsolutePath).Return("/one/two.aspx");
			Expect.On(context).Call(context.RawUrl).Return("/one/two.aspx");
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

			Expect.On(parser).Call(parser.Parse("/default.aspx?page=3")).Return(two);

			IWebContext context = mocks.CreateMock<IWebContext>();
			Expect.On(context).Call(context.AbsolutePath).Return("/default.aspx");
			Expect.On(context).Call(context.RawUrl).Return("/default.aspx?page=3");
			Expect.Call(delegate { context.CurrentPage = two; });

			mocks.ReplayAll();

			UrlRewriter rewriter = new UrlRewriter(parser, context);
			rewriter.UpdateCurrentPage();
		}

		[Test]
		[Ignore("Rewrite caching removed: dubious performance benefit")]
		public void DoesntCacheUrlParameters()
		{
			ContentItem root = CreateOneItem<PageItem>(1, "root", null);
			ContentItem one = CreateOneItem<PageItem>(2, "one", root);

			Expect.On(parser).Call(parser.Parse("/one.aspx")).Return(one).Repeat.Any();

			IWebContext context = MockContext(one, "happy=false");
			context.RewritePath("/default.aspx?page=2&happy=false");
			LastCall.Repeat.Once();

			//IWebContext context2 = MockContext(one, string.Empty);
			//context2.RewritePath("/default.aspx?page=2");
			//LastCall.Repeat.Once();

			mocks.ReplayAll();

			UrlRewriter rewriter = new UrlRewriter(parser, context);
			rewriter.RewriteRequest();
		}

		private IWebContext MockContext(ContentItem one, string query)
		{
			IWebContext context = mocks.CreateMock<IWebContext>();
			HttpRequest request = new HttpRequest(AppDomain.CurrentDomain.BaseDirectory + "test.aspx", "http://n2cms.com/one/two.aspx" + (query.Length > 0 ? "?" : string.Empty) + query, "");
			Expect.On(context).Call(context.Query).Return(query).Repeat.AtLeastOnce();
			Expect.On(context).Call(context.Request).Return(request).Repeat.Any();
			Expect.On(context).Call(context.RawUrl).Return("/one.aspx").Repeat.Any();
			return context;
		}
	}
}
