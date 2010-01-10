using System.Linq;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using Microsoft.Web.Mvc;
using N2.Configuration;
using N2.Definitions;
using N2.Engine;
using N2.Extensions.Tests.Fakes;
using N2.Extensions.Tests.Mvc.Controllers;
using N2.Extensions.Tests.Mvc.Models;
using N2.Persistence.NH;
using N2.Tests;
using N2.Web;
using N2.Web.Mvc;
using NUnit.Framework;
using Rhino.Mocks;
using HtmlHelper = System.Web.Mvc.HtmlHelper;

namespace N2.Extensions.Tests.Mvc
{
	[TestFixture]
	public class ContentRouteTests : N2.Tests.ItemPersistenceMockingBase
	{
		#region SetUp

		IEngine engine;
		FakeHttpContext httpContext;
		RequestContext requestContext;
		RouteCollection routes;
		IControllerMapper controllerMapper;
		
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
			typeFinder.typeMap[typeof(ContentItem)] = this.NearbyTypes()
				.BelowNamespace("N2.Extensions.Tests.Mvc.Models").AssignableTo<ContentItem>().Union(typeof(ContentItem)).ToArray();
			typeFinder.typeMap[typeof(IController)] = this.NearbyTypes()
				.BelowNamespace("N2.Extensions.Tests.Mvc.Controllers").AssignableTo<IController>().Except(typeof(AnotherRegularController))
				.ToArray();

			var definitions = new DefinitionManager(new DefinitionBuilder(typeFinder, new EngineSection()), new N2.Workflow.StateChanger(), null);
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
			httpContext.request.appRelativeCurrentExecutionFilePath = url;
			httpContext.request.rawUrl = url;
			var data = route.GetRouteData(httpContext);
			requestContext = new RequestContext(httpContext, data);
		}

		private RequestContext CreateRouteContext(ContentItem item)
		{
			SetPath(item.Url);

			var ctx = new RequestContext(httpContext, new RouteData());
			ctx.RouteData.Values[ContentRoute.ContentItemKey] = item;
			ctx.RouteData.Values[ContentRoute.ControllerKey] = controllerMapper.GetControllerName(item.GetType());
			return ctx;
		}
		#endregion

		// /			-> RegularPage
		// /about/		-> AboutUsSectionPage
		// /executives/ -> ExecutiveTeamPage
		// /search/		-> SearchPage

		[Test]
		public void CanFindController_ForContentPage()
		{
			SetPath("/about/");

			var data = route.GetRouteData(httpContext);

			Assert.That(data.Values[ContentRoute.ContentItemKey], Is.EqualTo(about));
			Assert.That(data.Values["controller"], Is.EqualTo("AboutUsSectionPage"));
			Assert.That(data.Values["action"], Is.EqualTo("index"));
		}

		[Test]
		public void CanFindController_DefaultAspx()
		{
			SetPath("/default.aspx");

			var data = route.GetRouteData(httpContext);

			Assert.That(data.Values[ContentRoute.ContentItemKey], Is.EqualTo(root));
			Assert.That(data.Values["controller"], Is.EqualTo("Regular"));
			Assert.That(data.Values["action"], Is.EqualTo("index"));
		}

		[Test]
		public void CanFindController_Slash()
		{
			SetPath("/");
			
			var data = route.GetRouteData(httpContext);

			Assert.That(data.Values[ContentRoute.ContentItemKey], Is.EqualTo(root));
			Assert.That(data.Values["controller"], Is.EqualTo("Regular"));
			Assert.That(data.Values["action"], Is.EqualTo("index"));
		}

		[Test]
		public void CanFindController_ForExtendingType()
		{
			SetPath("/about/executives/");

			var data = route.GetRouteData(httpContext);

			Assert.That(data.Values[ContentRoute.ContentItemKey], Is.EqualTo(executives));
			Assert.That(data.Values["controller"], Is.EqualTo("ExecutiveTeam"));
			Assert.That(data.Values["action"], Is.EqualTo("index"));
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
		public void GeneratesNullVirtualPathData_WhenLinkingTo_AVanillaMvcPage_FromAContentPage()
		{
			SetPath("/about/");

			var routeData = new RouteData();
			routeData.Values[ContentRoute.ContentItemKey] = new RegularPage();

			var requestContext = new RequestContext(httpContext, routeData);
			var virtualPath = route.GetVirtualPath(requestContext,
			                                       new RouteValueDictionary {{"controller", "Mvc"}, {"action", "Index"}});

			Assert.That(virtualPath, Is.Null);
		}

		[Test]
		public void GetVirtualPath_ToSelf()
		{
			SetPath("/search/");

			var vpd = route.GetVirtualPath(requestContext, new RouteValueDictionary());

			Assert.That(vpd.VirtualPath, Is.EqualTo("search"));
		}

		[Test]
		public void GetVirtualPath_ToOtherAction_OnSelf()
		{
			SetPath("/search/");

			var vpd = route.GetVirtualPath(requestContext, new RouteValueDictionary(new { action = "find" }));

			Assert.That(vpd.VirtualPath, Is.EqualTo("search/find"));
		}

		[Test]
		public void GetVirtualPath_ToOtherAction_OnSelf_WithParameter()
		{
			SetPath("/search/");

			var vpd = route.GetVirtualPath(requestContext, new RouteValueDictionary(new { action = "find", q = "query" }));

			Assert.That(vpd.VirtualPath, Is.EqualTo("search/find?q=query"));
		}

		[Test]
		public void GetVirtualPathTo_OtherContentItem()
		{
			SetPath("/search/");

			var vpd = route.GetVirtualPath(requestContext, new RouteValueDictionary(new { n2_item = executives }));

			Assert.That(vpd.VirtualPath, Is.EqualTo("about/executives"));
		}

		[Test]
		public void GetVirtualPath_ToAction_OnOtherContentItem()
		{
			SetPath("/about/executives/");

			var vpd = route.GetVirtualPath(requestContext, new RouteValueDictionary(new { n2_item = search, action = "find" }));

			Assert.That(vpd.VirtualPath, Is.EqualTo("search/find"));
		}

		[Test]
		public void GetVirtualPath_ToAction_OnOtherContentItem_WithParameters()
		{
			SetPath("/about/executives/");

			var vpd = route.GetVirtualPath(requestContext, new RouteValueDictionary(new { n2_item = search, action = "find", q = "what", x = "y" }));

			Assert.That(vpd.VirtualPath, Is.EqualTo("search/find?q=what&x=y"));
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

		// parts

		[Test]
		public void CanRoute_ToPart()
		{
			var part = CreateOneItem<TestItem>(10, "whatever", root);
			SetPath(part.Url);

			var r = routes.GetRouteData(httpContext);
			Assert.That(r.Values["controller"], Is.EqualTo("TestItem"));
		}

		[Test]
		public void GetVirtualPath_ToPartDefaultAction()
		{
			var part = CreateOneItem<TestItem>(10, "whatever", about);
			SetPath("/about/");

			var vpd = route.GetVirtualPath(requestContext, new RouteValueDictionary(new { n2_item = part }));

			Assert.That(vpd.VirtualPath, Is.EqualTo("about?item=10"));
		}

		[Test]
		public void GetVirtualPath_ToPart_OtherActionAction()
		{
			var part = CreateOneItem<TestItem>(10, "whatever", about);
			SetPath("/about/");

			var vpd = route.GetVirtualPath(requestContext, new RouteValueDictionary(new { n2_item = part, action = "doit" }));

			Assert.That(vpd.VirtualPath, Is.EqualTo("about/doit?item=10"));
		}

		[Test]
		public void CanCreate_ActionLink_ToPart()
		{
			var rc = CreateRouteContext(CreateOneItem<TestItem>(10, "whatever", root));
			var helper = new HtmlHelper(CreateViewContext(rc), new ViewPage(), routes);

			string html = helper.ActionLink("Hello", "Submit", new { q = "something", controller = "TestItem" });

			Assert.That(html, Is.EqualTo("<a href=\"/Submit?q=something&amp;item=10\">Hello</a>"));
		}

		ViewContext CreateViewContext(RequestContext rc)
		{
			return new ViewContext { RequestContext = rc, HttpContext = httpContext, RouteData = rc.RouteData };
		}
	}
}
