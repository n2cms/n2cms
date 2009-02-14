using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using N2.Configuration;
using N2.Definitions;
using N2.Engine;
using N2.Extensions.Tests.Mvc.Controllers;
using N2.Extensions.Tests.Mvc.Models;
using N2.Persistence;
using N2.Web.Mvc;
using NUnit.Framework;
using System.Web;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;
using N2.Web;
using System.Collections.Specialized;
using Microsoft.Web.Mvc;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using HtmlHelper = System.Web.Mvc.HtmlHelper;

namespace N2.Extensions.Tests.Mvc
{
	[TestFixture]
	public class ContentRouteTests : N2.Tests.ItemPersistenceMockingBase
	{
		static ContentRouteTests()
		{
			//RouteTable.Routes.Add(new ContentRoute(null, null));
		}

		IEngine engine;
		FakeHttpContext httpContext;
		RegularPage root;
		AboutUsSectionPage about;
		ExecutiveTeamPage executives;
		SearchPage search;
		ContentRoute route;
			
		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			root = CreateOneItem<RegularPage>(1, "root", null);
			about = CreateOneItem<AboutUsSectionPage>(2, "about", root);
			executives = CreateOneItem<ExecutiveTeamPage>(3, "executives", about);
			search = CreateOneItem<SearchPage>(4, "search", root);

			var typeFinder = new FakeTypeFinder();
			typeFinder.typeMap[typeof (ContentItem)] = new[]
			{
				typeof(RegularPage),
				typeof(AboutUsSectionPage),
				typeof(ExecutiveTeamPage),
				typeof(ContentItem),
				typeof(SearchPage)
			};
			typeFinder.typeMap[typeof (IController)] = new[]
			{
				typeof (ExecutiveTeamController),
				typeof (AboutUsSectionPageController), 
				typeof (RegularControllerBase), 
				typeof (FallbackContentController),
				typeof (NonN2Controller),
				typeof (SearchController)
			};

			var definitions = new DefinitionManager(new DefinitionBuilder(typeFinder), null);
			var webContext = new ThreadContext();
			var host = new Host(webContext, root.ID, root.ID);
			var parser = new UrlParser(persister, webContext, new ItemNotifier(), host, new HostSection());

			engine = mocks.DynamicMock<IEngine>();
			SetupResult.For(engine.Resolve<ITypeFinder>()).Return(typeFinder);
			SetupResult.For(engine.Definitions).Return(definitions);
			SetupResult.For(engine.UrlParser).Return(parser);
			engine.Replay();

			route = new ContentRoute(engine, new MvcRouteHandler());

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
			SetPath("/about/");

			var data = route.GetRouteData(httpContext);

			Assert.That(data.Values["ContentItem"], Is.EqualTo(about));
			Assert.That(data.Values["controller"], Is.EqualTo("AboutUsSectionPage"));
		}

		[Test]
		public void CanFindController_DefaultAspx()
		{
			SetPath("/default.aspx");

			var data = route.GetRouteData(httpContext);

			Assert.That(data.Values["ContentItem"], Is.EqualTo(root));
			Assert.That(data.Values["controller"], Is.EqualTo("Regular"));
		}

		[Test]
		public void CanFindController_Slash()
		{
			SetPath("/");
			
			var data = route.GetRouteData(httpContext);

			Assert.That(data.Values["ContentItem"], Is.EqualTo(root));
			Assert.That(data.Values["controller"], Is.EqualTo("Regular"));
		}

		[Test]
		public void CanFindController_ForExtendingType()
		{
			SetPath("/about/executives/");

			var data = route.GetRouteData(httpContext);

			Assert.That(data.Values["ContentItem"], Is.EqualTo(executives));
			Assert.That(data.Values["controller"], Is.EqualTo("ExecutiveTeam"));
		}

		[Test]
		public void WontGetVirtualPath_ForNonN2Controllers()
		{
			SetPath("/about/");

			RouteData routeData = new RouteData();
			routeData.Values["ContentItem"] = new RegularPage();
			var data = route.GetVirtualPath(
				new RequestContext(httpContext, routeData), 
				new RouteValueDictionary { {"controller", "NonN2"} });

			Assert.That(data, Is.Null);
		}

		[Test]
		public void CanGetActionNameFromUrl_WithoutUsingTheQueryString()
		{
			SetPath("/about/submit");

			var routeData = route.GetRouteData(httpContext);

			Assert.That(routeData, Is.Not.Null);
			Assert.That(routeData.Values["controller"], Is.EqualTo("AboutUsSectionPage"));
			Assert.That(routeData.Values["action"], Is.EqualTo("submit"));
		}

		[Test]
		public void GeneratesNullVirtualPathData_WhenLinkingToAVanillaMvcPageFromAContentPage()
		{
			SetPath("/about/");

			var routeData = new RouteData();
			routeData.Values["ContentItem"] = new RegularPage();

			var requestContext = new RequestContext(httpContext, routeData);
			var virtualPath = route.GetVirtualPath(requestContext,
			                                       new RouteValueDictionary {{"controller", "Mvc"}, {"action", "Index"}});

			Assert.That(virtualPath, Is.Null);
		}

		[Test, Ignore]
		public void CanCreate_RouteLink()
		{
			SetPath("/search/");
			HtmlHelper helper = new HtmlHelper(new ViewContext(), new ViewPage());

			string html = helper.RouteLink("Search", new {controller = "Search", q = "hello"});

			Assert.That(html, Is.EqualTo("<a href=\"/search?q=hello\">Search</a>"));
		}

		[Test, Ignore]
		public void Can_GenerateRouteLink()
		{
			SetPath("/search/");

			var rc = new RequestContext(new FakeHttpContext(), new RouteData());
			string html = HtmlHelper.GenerateRouteLink(rc, new RouteCollection(), "Search", null, new RouteValueDictionary(new {controller = "Search", q = "hello"}), new Dictionary<string, object>());

			Assert.That(html, Is.EqualTo("<a href=\"/search?q=hello\">Search</a>"));
		}

		[Test, Ignore]
		public void CanCreate_ActionLink()
		{
			SetPath("/search/");
			HtmlHelper helper = new HtmlHelper(new ViewContext(), new ViewPage());

			string url1 = helper.ActionLink("Hello", "Search", new {q = "query"});

			Assert.That(url1, Is.EqualTo("/search?q=hello"));
		}

		[Test, Ignore]
		public void CanCreate_UrlFromExpression()
		{
			SetPath("/search/");
			HtmlHelper helper = new HtmlHelper(new ViewContext(), new ViewPage());

			string url2 = helper.BuildUrlFromExpression<SearchController>(s => s.Search("hello"));

			Assert.That(url2, Is.EqualTo("/search?q=hello"));
		}

		//TODO figure out
		//[Test]
		//public void InstantiatesTheCorrectController()
		//{
		//    SetPath("/");

		//    var data = route.GetRouteData(httpContext);


		//    var factory = new DefaultControllerFactory();
		//    var requestContext = new RequestContext(httpContext, data);
		//    factory.CreateController(requestContext, (string)data.Values["controller"]);
		//}

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
