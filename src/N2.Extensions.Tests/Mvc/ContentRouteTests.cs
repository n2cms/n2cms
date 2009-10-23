using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using N2.Configuration;
using N2.Definitions;
using N2.Engine;
using N2.Extensions.Tests.Fakes;
using N2.Extensions.Tests.Mvc.Controllers;
using N2.Extensions.Tests.Mvc.Models;
using N2.Persistence;
using N2.Persistence.NH;
using N2.Web.Mvc;
using NUnit.Framework;
using Rhino.Mocks;
using N2.Web;
using Microsoft.Web.Mvc;
using System.Web.Mvc.Html;
using HtmlHelper = System.Web.Mvc.HtmlHelper;
using System.Diagnostics;

namespace N2.Extensions.Tests.Mvc
{
	[TestFixture]
	public class ContentRouteTests : N2.Tests.ItemPersistenceMockingBase
	{
		IEngine engine;
		FakeHttpContext httpContext;
		RegularPage root;
		AboutUsSectionPage about;
		ExecutiveTeamPage executives;
		SearchPage search;
		ContentRoute route;
		RouteCollection routes;
		IControllerMapper controllerMapper;
			
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
				typeof(SearchPage),
				typeof(TestItem),
			};
			typeFinder.typeMap[typeof (IController)] = new[]
			{
				typeof (ExecutiveTeamController),
				typeof (AboutUsSectionPageController), 
				typeof (RegularController), 
				typeof (FallbackContentController),
				typeof (NonN2Controller),
				typeof (SearchController),
				typeof (TestItemController),
			};

			var definitions = new DefinitionManager(new DefinitionBuilder(typeFinder, new EngineSection()), null);
			var webContext = new ThreadContext();
			var host = new Host(webContext, root.ID, root.ID);
			var parser = new UrlParser(persister, webContext, new NotifyingInterceptor(), host, new HostSection());
			controllerMapper = new ControllerMapper(typeFinder, definitions);
			Url.DefaultExtension = "";

			engine = mocks.DynamicMock<IEngine>();
			SetupResult.For(engine.Resolve<ITypeFinder>()).Return(typeFinder);
			SetupResult.For(engine.Definitions).Return(definitions);
			SetupResult.For(engine.UrlParser).Return(parser);
			engine.Replay();

			route = new ContentRoute(engine, new MvcRouteHandler(), controllerMapper);

			httpContext = new FakeHttpContext();
			routes = new RouteCollection { route };
		}

		void SetPath(string url)
		{
			Debug.WriteLine("Setting path to " + url);
			httpContext.request.appRelativeCurrentExecutionFilePath = url;
			httpContext.request.rawUrl = url;
		}

		private RequestContext CreateRouteContext(ContentItem item)
		{
			SetPath(item.Url);

			var ctx = new RequestContext(httpContext, new RouteData());
			ctx.RouteData.Values[ContentRoute.ContentItemKey] = item;
			ctx.RouteData.Values[ContentRoute.ControllerKey] = controllerMapper.GetControllerName(item.GetType());
			return ctx;
		}

		[Test]
		public void CanFindController_ForContentPage()
		{
			SetPath("/about/");

			var data = route.GetRouteData(httpContext);

			Assert.That(data.Values[ContentRoute.ContentItemKey], Is.EqualTo(about));
			Assert.That(data.Values["controller"], Is.EqualTo("AboutUsSectionPage"));
		}

		[Test]
		public void CanFindController_DefaultAspx()
		{
			SetPath("/default.aspx");

			var data = route.GetRouteData(httpContext);

			Assert.That(data.Values[ContentRoute.ContentItemKey], Is.EqualTo(root));
			Assert.That(data.Values["controller"], Is.EqualTo("Regular"));
		}

		[Test]
		public void CanFindController_Slash()
		{
			SetPath("/");
			
			var data = route.GetRouteData(httpContext);

			Assert.That(data.Values[ContentRoute.ContentItemKey], Is.EqualTo(root));
			Assert.That(data.Values["controller"], Is.EqualTo("Regular"));
		}

		[Test]
		public void CanFindController_ForExtendingType()
		{
			SetPath("/about/executives/");

			var data = route.GetRouteData(httpContext);

			Assert.That(data.Values[ContentRoute.ContentItemKey], Is.EqualTo(executives));
			Assert.That(data.Values["controller"], Is.EqualTo("ExecutiveTeam"));
		}

		[Test]
		public void WontGetVirtualPath_ForNonN2Controllers()
		{
			SetPath("/about/");

			RouteData routeData = new RouteData();
			routeData.Values[ContentRoute.ContentItemKey] = new RegularPage();
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
			routeData.Values[ContentRoute.ContentItemKey] = new RegularPage();

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

		[Test]
		public void CanGenerate_DefaultRouteUrl()
		{
			var rc = CreateRouteContext(search);
			var helper = new UrlHelper(rc, routes);

			string url = helper.RouteUrl(null, new { controller = "Search" }, null);

			Assert.That(url, Is.EqualTo("/search"));
		}

		[Test]
		public void CanGenerateLink()
		{
			var rc = CreateRouteContext(search);

			string html = HtmlHelper.GenerateLink(rc, routes, "Hello", null, "find", "Search", new RouteValueDictionary(new { q = "hello" }), null);

			Assert.That(html, Is.EqualTo("<a href=\"/search/find?q=hello\">Hello</a>"));
		}

		[Test]
		public void CanCreate_ActionLink()
		{
			var rc = CreateRouteContext(search);
			var helper = new HtmlHelper(CreateViewContext(rc), new ViewPage(), routes);

			string html = helper.ActionLink("Hello", "find", new { q = "something", controller = "Search" });

			Assert.That(html, Is.EqualTo("<a href=\"/search/find?q=something\">Hello</a>"));
		}

		[Test]
		public void CanCreate_UrlFromExpression()
		{
			var rc = CreateRouteContext(search);
			var helper = new HtmlHelper(CreateViewContext(rc), new ViewPage(), routes);

			string html = helper.BuildUrlFromExpression<SearchController>(s => s.Find("hello"));

			Assert.That(html, Is.EqualTo("/search/Find?q=hello"));
		}

		[Test]
		public void CanCreate_ActionLinkFromItem()
		{
			var rc = CreateRouteContext(new TestItem { ID = 10 });
			var helper = new HtmlHelper(CreateViewContext(rc), new ViewPage(), routes);

			string html = helper.ActionLink("Hello", "Submit", new { q = "something", controller = "TestItem" });

			Assert.That(html, Is.EqualTo("<a href=\"/TestItem/Submit?q=something&amp;n2_itemid=10\">Hello</a>"));
		}

		ViewContext CreateViewContext(RequestContext rc)
		{
			return new ViewContext { RequestContext = rc, HttpContext = httpContext, RouteData = rc.RouteData };
		}
	}
}
