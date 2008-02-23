using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using MbUnit.Framework;
using N2.Persistence;
using N2.Web;
using Rhino.Mocks;

namespace N2.Tests.Web
{
	[TestFixture]
	public class UrlRewriterTests : ItemTestsBase
	{
		UrlRewriter rewriter;
		IUrlParser parser;
			
		[SetUp]
		public override void SetUp()
		{
			base.SetUp();
			IPersister persister = mocks.Stub<IPersister>();
			parser = mocks.CreateMock<IUrlParser>();
			Expect.On(parser).Call(parser.DefaultExtension).Return(".aspx").Repeat.AtLeastOnce();
			rewriter = new UrlRewriter(persister, parser);
		}

		[Test]
		public void CanRewriteUrl()
		{
			ContentItem root = CreateOneItem<PageItem>(1, "root", null);
			ContentItem one = CreateOneItem<PageItem>(2, "one", root);
			ContentItem two = CreateOneItem<PageItem>(3, "two", one);

			IWebContext context = mocks.CreateMock<IWebContext>();
			HttpRequest request = new HttpRequest(AppDomain.CurrentDomain.BaseDirectory + "test.aspx", "http://n2cms.com/one/two.aspx", "");
			Expect.On(context).Call(context.QueryString).Return(string.Empty).Repeat.Once(); 
			Expect.On(context).Call(context.Request).Return(request).Repeat.Any();
			Expect.On(context).Call(context.RelativeUrl).Return("/one/two.aspx").Repeat.Any();
			Expect.On(parser).Call(parser.Parse("/one/two.aspx")).Return(two);
			context.RewritePath("/default.aspx?page=3");
			LastCall.Repeat.Once();
			mocks.ReplayAll();

			rewriter.RewriteRequest(context);
		}

		[Test]
		public void RewriteUrlAppendsExistingQueryString()
		{
			ContentItem root = CreateOneItem<PageItem>(1, "root", null);
			ContentItem one = CreateOneItem<PageItem>(2, "one", root);
			ContentItem two = CreateOneItem<PageItem>(3, "two", one);

			IWebContext context = mocks.CreateMock<IWebContext>();
			HttpRequest request = new HttpRequest(AppDomain.CurrentDomain.BaseDirectory + "test.aspx", "http://n2cms.com/one/two.aspx?happy=true&flip=feet", "");
			Expect.On(context).Call(context.QueryString).Return("happy=true&flip=feet").Repeat.Twice();
			Expect.On(context).Call(context.Request).Return(request).Repeat.Any();
			Expect.On(context).Call(context.RelativeUrl).Return("/one/two.aspx").Repeat.Any();
			Expect.On(parser).Call(parser.Parse("/one/two.aspx")).Return(two);
			context.RewritePath("/default.aspx?page=3&happy=true&flip=feet");
			LastCall.Repeat.Once();
			mocks.ReplayAll();

			rewriter.RewriteRequest(context);
		}

		[Test]
		public void DoesntCacheUrlParameters()
		{
			ContentItem root = CreateOneItem<PageItem>(1, "root", null);
			ContentItem one = CreateOneItem<PageItem>(2, "one", root);

			Expect.On(parser).Call(parser.Parse("/one.aspx")).Return(one).Repeat.Any();
			
			IWebContext context1 = MockContext(one, "happy=false");
			context1.RewritePath("/default.aspx?page=2&happy=false");
			LastCall.Repeat.Once();

			IWebContext context2 = MockContext(one, string.Empty);
			context2.RewritePath("/default.aspx?page=2");
			LastCall.Repeat.Once();

			mocks.ReplayAll();

			rewriter.RewriteRequest(context1);
			rewriter.RewriteRequest(context2);
		}

		private IWebContext MockContext(ContentItem one, string query)
		{
			IWebContext context = mocks.CreateMock<IWebContext>();
			HttpRequest request = new HttpRequest(AppDomain.CurrentDomain.BaseDirectory + "test.aspx", "http://n2cms.com/one/two.aspx" + (query.Length > 0 ? "?" : string.Empty) + query, "");
			Expect.On(context).Call(context.QueryString).Return(query).Repeat.AtLeastOnce();
			Expect.On(context).Call(context.Request).Return(request).Repeat.Any();
			Expect.On(context).Call(context.RelativeUrl).Return("/one.aspx").Repeat.Any();
			return context;
		}
	}
}
