using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using N2.Definitions;
using N2.Engine;
using N2.Persistence;
using N2.Persistence.NH;
using N2.Tests.Fakes;
using N2.Web.Mvc;
using NUnit.Framework;
using System.Web;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;
using N2.Web;
using N2.Details;
using System.Collections.Specialized;

namespace N2.Extensions.Tests.Mvc
{
	[TestFixture]
	public class ContentRouteTests : N2.Tests.ItemPersistenceMockingBase
	{
		RouteCollection routes;
		IEngine engine;
		FakeHttpContext httpContext;
		PageItem root, item1;
		NewsItem news1;
		ContentRoute route;
			
		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			root = CreateOneItem<PageItem>(1, "root", null);
			item1 = CreateOneItem<PageItem>(2, "item1", root);
			news1 = CreateOneItem<NewsItem>(3, "news1", root);

			var typeFinder = new FakeTypeFinder();
			typeFinder.typeMap[typeof (ContentItem)] = new[]
			{
				typeof(BaseItem),
				typeof(PageItem),
				typeof(NewsItem),
				typeof(ContentItem)
			};
			typeFinder.typeMap[typeof (IController)] = new[]
			{
				typeof (NewsController),
				typeof (PageController), 
				typeof (BaseController), 
				typeof (FallbackContentController)
			};

			var definitions = new DefinitionManager(new DefinitionBuilder(typeFinder), null);
			var webContext = new ThreadContext();
			var persister = new ContentPersister(repository, null, null);
			var parser = new UrlParser(persister, webContext, new ItemNotifier(), new Host(webContext, root.ID, root.ID));

			engine = mocks.DynamicMock<IEngine>();
			SetupResult.For(engine.Resolve<ITypeFinder>()).Return(typeFinder);
			SetupResult.For(engine.Definitions).Return(definitions);
			SetupResult.For(engine.UrlParser).Return(parser);
			engine.Replay();

			routes = new RouteCollection();

			route = new ContentRoute(engine, new MvcRouteHandler());
			routes.Add(route);

			httpContext = new FakeHttpContext();
			httpContext.request = new FakeHttpRequest();
		}

		void SetPath(string url)
		{
			httpContext.request.appRelativeCurrentExecutionFilePath = url;
			httpContext.request.rawUrl = url;
		}

		[Test]
		public void CanFindController_ForContentPage()
		{
			SetPath("/");
			
			var data = routes.GetRouteData(httpContext);

			Assert.That(data.Values["ContentItem"], Is.EqualTo(root));
			Assert.That(data.Values["controller"], Is.EqualTo("Page"));
		}

		[Test]
		public void CanFindController_ForExtendingType()
		{
			SetPath("/news1/");

			var data = route.GetRouteData(httpContext);

			Assert.That(data.Values["ContentItem"], Is.EqualTo(news1));
			Assert.That(data.Values["controller"], Is.EqualTo("News"));
		}

		public class FakeHttpContext : HttpContextBase
		{
			public FakeHttpRequest request;
			public override HttpRequestBase Request
			{
				get { return request; }
			}
		}
		public class FakeHttpRequest : HttpRequestBase
		{
			public string appRelativeCurrentExecutionFilePath;
			public override string AppRelativeCurrentExecutionFilePath
			{
				get { return appRelativeCurrentExecutionFilePath; }
			}
			public string rawUrl;
			public override string RawUrl
			{
				get { return rawUrl; }
			}
			public StringDictionary query = new StringDictionary();
			public override string this[string key]
			{
				get { return query[key]; }
			}
		}
		public class FakeTypeFinder : AppDomainTypeFinder
		{
			public Dictionary<Type, IList<Type>> typeMap = new Dictionary<Type, IList<Type>>();
			public override IList<Type> Find(Type requestedType)
			{
				if (typeMap.ContainsKey(requestedType))
					return typeMap[requestedType];
				return base.Find(requestedType);
			}
		}
	}
}
