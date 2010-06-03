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
using System.Collections.Specialized;
using System.Collections.Generic;

namespace N2.Extensions.Tests.Mvc
{
	[TestFixture]
	public class ContentRouteTests : N2.Tests.ItemPersistenceMockingBase
	{
		#region SetUp

		IEngine engine;
		FakeHttpContext httpContext;
		RequestContext requestContext = null;
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

			var definitions = new DefinitionManager(new DefinitionBuilder(typeFinder, new EngineSection()), new N2.Edit.Workflow.StateChanger(), null);
			var webContext = new ThreadContext();
			var host = new Host(webContext, root.ID, root.ID);
			var parser = new UrlParser(persister, webContext, new NotifyingInterceptor(), host, new HostSection());
			controllerMapper = new ControllerMapper(typeFinder, definitions);
			Url.DefaultExtension = "";

			engine = mocks.DynamicMock<IEngine>();
			SetupResult.For(engine.Resolve<ITypeFinder>()).Return(typeFinder);
			SetupResult.For(engine.Definitions).Return(definitions);
			SetupResult.For(engine.UrlParser).Return(parser);
			SetupResult.For(engine.Persister).Return(persister);
			engine.Replay();

			route = new ContentRoute(engine, new MvcRouteHandler(), controllerMapper, null);

			httpContext = new FakeHttpContext();
			routes = new RouteCollection { route };
		}

		#endregion

		// /					-> RegularPage
		// /about/				-> AboutUsSectionPage
		// /about/executives/	-> ExecutiveTeamPage
		// /search/				-> SearchPage

		/*** GetRouteData ***/

		[Test]
		public void CanFindController_ForStartPage()
		{
			var data = RoutePath("/");

			Assert.That(data.DataTokens[ContentRoute.ContentItemKey], Is.EqualTo(root));
			Assert.That(data.DataTokens[ContentRoute.ContentPageKey], Is.EqualTo(root));
			Assert.That(data.Values[ContentRoute.ContentItemKey], Is.EqualTo(root.ID));
			Assert.That(data.Values[ContentRoute.ContentPageKey], Is.EqualTo(root.ID));
			Assert.That(data.Values["controller"], Is.EqualTo("Regular"));
			Assert.That(data.Values["action"], Is.EqualTo("index"));
		}

		[Test]
		public void CanFindController_ForStartPage_default_aspx()
		{
			var data = RoutePath("/default.aspx");

			Assert.That(data.DataTokens[ContentRoute.ContentItemKey], Is.EqualTo(root));
			Assert.That(data.DataTokens[ContentRoute.ContentPageKey], Is.EqualTo(root));
			Assert.That(data.Values[ContentRoute.ContentItemKey], Is.EqualTo(root.ID));
			Assert.That(data.Values[ContentRoute.ContentPageKey], Is.EqualTo(root.ID));
			Assert.That(data.Values["controller"], Is.EqualTo("Regular"));
			Assert.That(data.Values["action"], Is.EqualTo("index"));
		}

		[Test]
		public void CanFindController_ForContentPage()
		{
			var data = RoutePath("/about/");

			Assert.That(data.DataTokens[ContentRoute.ContentItemKey], Is.EqualTo(about));
			Assert.That(data.DataTokens[ContentRoute.ContentPageKey], Is.EqualTo(about));
			Assert.That(data.Values[ContentRoute.ContentItemKey], Is.EqualTo(about.ID));
			Assert.That(data.Values[ContentRoute.ContentPageKey], Is.EqualTo(about.ID));
			Assert.That(data.Values["controller"], Is.EqualTo("AboutUsSectionPage"));
			Assert.That(data.Values["action"], Is.EqualTo("index"));
		}

		[Test]
		public void CanFindController_ForContentPage_NoSlash()
		{
			var data = RoutePath("/about");

			Assert.That(data.DataTokens[ContentRoute.ContentItemKey], Is.EqualTo(about));
			Assert.That(data.Values["controller"], Is.EqualTo("AboutUsSectionPage"));
			Assert.That(data.Values["action"], Is.EqualTo("index"));
		}

		[Test]
		public void RoutesToPage_WhenPart_IsPassedAsItem_InQuery()
		{
			var part = CreateOneItem<TestItem>(10, "whatever", root);
			var data = RoutePath("/about/?item=10");

			Assert.That(data.DataTokens[ContentRoute.ContentPageKey], Is.EqualTo(about));
			Assert.That(data.DataTokens[ContentRoute.ContentItemKey], Is.EqualTo(about));
			Assert.That(data.DataTokens[ContentRoute.ContentPartKey], Is.EqualTo(part));
			Assert.That(data.Values[ContentRoute.ContentPageKey], Is.EqualTo(about.ID));
			Assert.That(data.Values[ContentRoute.ContentItemKey], Is.EqualTo(about.ID));
			Assert.That(data.Values[ContentRoute.ContentPartKey], Is.EqualTo(part.ID));
			Assert.That(data.Values["controller"], Is.EqualTo("AboutUsSectionPage"));
			Assert.That(data.Values["action"], Is.EqualTo("index"));
		}

		[Test]
		public void CanFindController_ForExtendingType()
		{
			RoutePath("/about/executives/");

			var data = route.GetRouteData(httpContext);

			Assert.That(data.DataTokens[ContentRoute.ContentItemKey], Is.EqualTo(executives));
			Assert.That(data.Values["controller"], Is.EqualTo("ExecutiveTeam"));
			Assert.That(data.Values["action"], Is.EqualTo("index"));
		}

		[Test]
		public void CanGet_ActionName_FromUrl()
		{
			RoutePath("/about/submit");

			var routeData = route.GetRouteData(httpContext);

			Assert.That(routeData, Is.Not.Null);
			Assert.That(routeData.Values["controller"], Is.EqualTo("AboutUsSectionPage"));
			Assert.That(routeData.Values["action"], Is.EqualTo("submit"));
		}

		[Test, Ignore("TODO")]
		public void CanGet_IdParameter_FromUrl()
		{
			RoutePath("/about/submit/123");

			var routeData = route.GetRouteData(httpContext);

			Assert.That(routeData, Is.Not.Null);
			Assert.That(routeData.Values["controller"], Is.EqualTo("AboutUsSectionPage"));
			Assert.That(routeData.Values["action"], Is.EqualTo("submit"));
			Assert.That(routeData.Values["id"], Is.EqualTo("123"));
		}

		/*** GetVirtualPath ***/

		[Test]
		public void VirtualPath_IsNull_ForVanillaController()
		{
			RoutePath("/about/");

			var routeData = new RouteData();
			routeData.DataTokens[ContentRoute.ContentItemKey] = CreateOneItem<RegularPage>(99, "hello", root);
			routeData.DataTokens[ContentRoute.ContentPageKey] = routeData.DataTokens[ContentRoute.ContentItemKey];

			var requestContext = new RequestContext(httpContext, routeData);
			var virtualPath = route.GetVirtualPath(requestContext, new RouteValueDictionary(new { controller = "Mvc", action = "Index" }));

			Assert.That(virtualPath, Is.Null);
		}

		[Test]
		public void AppendsActionName_ToContentUrl()
		{
			RoutePath("/about/");

			UrlHelper url = new UrlHelper(requestContext, routes);

			var result = url.Action("hello");
			Assert.That(result, Is.EqualTo("/about/hello"));
		}

		[Test]
		public void DoesntRoute_ControllerOnly()
		{
			RoutePath("/about/");

			UrlHelper url = new UrlHelper(requestContext, routes);

			var result = url.Action("hello", "other");
			Assert.That(result, Is.Null);
		}

		[Test]
		public void CanCreate_ActionUrl_ToPage()
		{
			RoutePath("/about/");

			UrlHelper url = new UrlHelper(requestContext, routes);

			var result = url.Action("index", new { item = executives });
			Assert.That(result, Is.EqualTo("/about/executives"));
		}

		[Test]
		public void GetVirtualPath_ToSelf()
		{
			RoutePath("/search/");

			var vpd = route.GetVirtualPath(requestContext, new RouteValueDictionary());

			Assert.That(vpd.VirtualPath, Is.EqualTo("search"));
		}

		[Test]
		public void GetVirtualPath_ToOtherAction_OnSelf()
		{
			RoutePath("/search/");

			var vpd = route.GetVirtualPath(requestContext, new RouteValueDictionary(new { action = "find" }));

			Assert.That(vpd.VirtualPath, Is.EqualTo("search/find"));
		}

		[Test]
		public void GetVirtualPath_ToOtherAction_OnSelf_WithParameter()
		{
			RoutePath("/search/");

			var vpd = route.GetVirtualPath(requestContext, new RouteValueDictionary(new { action = "find", q = "query" }));

			Assert.That(vpd.VirtualPath, Is.EqualTo("search/find?q=query"));
		}

		[Test]
		public void GetVirtualPathTo_OtherContentItem()
		{
			RoutePath("/search/");

			var vpd = route.GetVirtualPath(requestContext, new RouteValueDictionary(new { item = executives }));

			Assert.That(vpd.VirtualPath, Is.EqualTo("about/executives"));
		}

		[Test]
		public void GetVirtualPathTo_OtherContentItem_ViaPage()
		{
			RoutePath("/search/");

			var vpd = route.GetVirtualPath(requestContext, new RouteValueDictionary(new { page = executives }));

			Assert.That(vpd.VirtualPath, Is.EqualTo("about/executives"));
		}

		[Test]
		public void GetVirtualPathTo_SameController_IsNotNull()
		{
			RoutePath("/about/");

			var vpd = route.GetVirtualPath(requestContext, new RouteValueDictionary(new { controller = "aboutussectionpage" }));

			Assert.That(vpd, Is.Not.Null);
			Assert.That(vpd.VirtualPath, Is.EqualTo("about"));
		}

		[Test]
		public void GetVirtualPathTo_SameController_AddsAction()
		{
			RoutePath("/about/");

			var vpd = route.GetVirtualPath(requestContext, new RouteValueDictionary(new { controller = "aboutussectionpage", action = "more" }));

			Assert.That(vpd, Is.Not.Null);
			Assert.That(vpd.VirtualPath, Is.EqualTo("about/more"));
		}

		[Test]
		public void GetVirtualPathTo_OtherController_IsNull()
		{
			RoutePath("/about/");

			var vpd = route.GetVirtualPath(requestContext, new RouteValueDictionary(new { controller = "hello", action = "index" }));

			Assert.That(vpd, Is.Null);
		}

		[Test]
		public void GetVirtualPath_ToAction_OnOtherContentItem()
		{
			RoutePath("/about/executives/");

			var vpd = route.GetVirtualPath(requestContext, new RouteValueDictionary(new { item = search, action = "find" }));

			Assert.That(vpd.VirtualPath, Is.EqualTo("search/find"));
		}

		[Test]
		public void GetVirtualPath_ToAction_OnOtherContentItem_WithParameters()
		{
			RoutePath("/about/executives/");

			var vpd = route.GetVirtualPath(requestContext, new RouteValueDictionary(new { item = search, action = "find", q = "what", x = "y" }));

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

			var html = helper.ActionLink("Hello", "find", new { q = "something", controller = "Search" });

			Assert.That(html.ToString(), Is.EqualTo("<a href=\"/search/find?q=something\">Hello</a>"));
		}

		[Test]
		public void CanCreate_UrlFromExpression()
		{
			var rc = CreateRouteContext(search);
			var helper = new HtmlHelper(CreateViewContext(rc), new ViewPage(), routes);

			string html = helper.BuildUrlFromExpression<SearchController>(s => s.Find("hello"));

			Assert.That(html, Is.EqualTo("/search/Find?q=hello"));
		}

		/*** GetRouteData parts ***/

		[Test]
		public void CanRoute_ToPart()
		{
			var part = CreateOneItem<TestItem>(10, "whatever", root);
			RoutePath("/TestItem/?part=10");

			var r = routes.GetRouteData(httpContext);
			Assert.That(r.Values[ContentRoute.ControllerKey], Is.EqualTo("TestItem"));
			Assert.That(r.Values[ContentRoute.ActionKey], Is.EqualTo("Index"));
			Assert.That(r.Values[ContentRoute.ContentPartKey], Is.EqualTo(part.ID));
			Assert.That(r.Values[ContentRoute.ContentItemKey], Is.EqualTo(part.ID));
			Assert.That(r.Values[ContentRoute.ContentPageKey], Is.EqualTo(root.ID));
			Assert.That(r.DataTokens[ContentRoute.ContentPartKey], Is.EqualTo(part));
			Assert.That(r.DataTokens[ContentRoute.ContentItemKey], Is.EqualTo(part));
			Assert.That(r.DataTokens[ContentRoute.ContentPageKey], Is.EqualTo(root));
		}

		[Test]
		public void CanRoute_ToPart_WithPage()
		{
			var part = CreateOneItem<TestItem>(10, "whatever", root);
			RoutePath("/TestItem/?page=1&part=10");

			var r = routes.GetRouteData(httpContext);
			Assert.That(r.Values[ContentRoute.ControllerKey], Is.EqualTo("TestItem"));
			Assert.That(r.Values[ContentRoute.ActionKey], Is.EqualTo("Index"));
			Assert.That(r.Values[ContentRoute.ContentPartKey], Is.EqualTo(part.ID));
			Assert.That(r.Values[ContentRoute.ContentItemKey], Is.EqualTo(part.ID));
			Assert.That(r.Values[ContentRoute.ContentPageKey], Is.EqualTo(root.ID));
			Assert.That(r.DataTokens[ContentRoute.ContentPartKey], Is.EqualTo(part));
			Assert.That(r.DataTokens[ContentRoute.ContentItemKey], Is.EqualTo(part));
			Assert.That(r.DataTokens[ContentRoute.ContentPageKey], Is.EqualTo(root));
		}

		[Test]
		public void CanRoute_ToPage_ViaController()
		{
			var part = CreateOneItem<TestItem>(10, "whatever", root);
			RoutePath("/Regular/?page=1");

			var r = routes.GetRouteData(httpContext);

			Assert.That(r.Values[ContentRoute.ControllerKey], Is.EqualTo("Regular"));
			Assert.That(r.Values[ContentRoute.ActionKey], Is.EqualTo("index"));
			Assert.That(r.Values[ContentRoute.ContentPageKey], Is.EqualTo(root.ID));
			Assert.That(r.Values[ContentRoute.ContentItemKey], Is.EqualTo(root.ID));
			Assert.That(r.Values[ContentRoute.ContentPartKey], Is.Null);
			Assert.That(r.DataTokens[ContentRoute.ContentPageKey], Is.EqualTo(root));
			Assert.That(r.DataTokens[ContentRoute.ContentItemKey], Is.EqualTo(root));
			Assert.That(r.DataTokens[ContentRoute.ContentPartKey], Is.Null);
		}

		[Test]
		public void CanRoute_ToPart_Action()
		{
			var part = CreateOneItem<TestItem>(10, "whatever", root);
			RoutePath("/TestItem/Submit/?part=10");

			var r = routes.GetRouteData(httpContext);
			Assert.That(r.Values[ContentRoute.ControllerKey], Is.EqualTo("TestItem"));
			Assert.That(r.Values[ContentRoute.ActionKey], Is.EqualTo("Submit"));
			Assert.That(r.Values[ContentRoute.ContentPartKey], Is.EqualTo(part.ID));
			Assert.That(r.Values[ContentRoute.ContentItemKey], Is.EqualTo(part.ID));
			Assert.That(r.Values[ContentRoute.ContentPageKey], Is.EqualTo(root.ID));
			Assert.That(r.DataTokens[ContentRoute.ContentPartKey], Is.EqualTo(part));
			Assert.That(r.DataTokens[ContentRoute.ContentItemKey], Is.EqualTo(part));
			Assert.That(r.DataTokens[ContentRoute.ContentPageKey], Is.EqualTo(root));
		}

		[Test]
		public void CanRoute_ToPart_Action_WithPage()
		{
			var part = CreateOneItem<TestItem>(10, "whatever", root);
			RoutePath("/TestItem/Submit/?page=1&part=10");

			var r = routes.GetRouteData(httpContext);
			Assert.That(r.Values[ContentRoute.ControllerKey], Is.EqualTo("TestItem"));
			Assert.That(r.Values[ContentRoute.ActionKey], Is.EqualTo("Submit"));
			Assert.That(r.Values[ContentRoute.ContentPartKey], Is.EqualTo(part.ID));
			Assert.That(r.Values[ContentRoute.ContentItemKey], Is.EqualTo(part.ID));
			Assert.That(r.Values[ContentRoute.ContentPageKey], Is.EqualTo(root.ID));
			Assert.That(r.DataTokens[ContentRoute.ContentPartKey], Is.EqualTo(part));
			Assert.That(r.DataTokens[ContentRoute.ContentItemKey], Is.EqualTo(part));
			Assert.That(r.DataTokens[ContentRoute.ContentPageKey], Is.EqualTo(root));
		}

		[Test]
		public void RoutingToPart_ViaItsVirtualPath_PassesController_OfThePart()
		{
			RoutePath("/");

			var part = CreateOneItem<TestItem>(10, "whatever", root);
			var vpd = routes.GetVirtualPath(requestContext, new RouteValueDictionary(new { item = part }));
			RoutePath(vpd.VirtualPath);

			var r = routes.GetRouteData(httpContext);
			Assert.That(r.Values[ContentRoute.ControllerKey], Is.EqualTo("TestItem"));
		}

		[Test]
		public void RoutingToPart_ViaItsVirtualPath_PassesAssociatedPart_AsContentPart()
		{
			RoutePath("/");

			var part = CreateOneItem<TestItem>(10, "whatever", root);
			var vpd = routes.GetVirtualPath(requestContext, new RouteValueDictionary(new { item = part }));
			RoutePath(vpd.VirtualPath);

			var r = routes.GetRouteData(httpContext);
			Assert.That(r.Values[ContentRoute.ContentPartKey], Is.EqualTo(part.ID));
			Assert.That(r.DataTokens[ContentRoute.ContentPartKey], Is.EqualTo(part));
		}

		[Test]
		public void RoutingToPart_PassesPart_AsContentPart()
		{
			var part = CreateOneItem<TestItem>(10, "whatever", root);
			var r = RoutePath("/TestItem/?part=10");

			Assert.That(r.Values[ContentRoute.ContentPartKey], Is.EqualTo(part.ID));
			Assert.That(r.DataTokens[ContentRoute.ContentPartKey], Is.EqualTo(part));
		}

		[Test]
		public void RoutingToPart_PassesAssociatedPage_AsContentPage()
		{
			var part = CreateOneItem<TestItem>(10, "whatever", root);
			var r = RoutePath("/TestItem/?part=10");

			Assert.That(r.Values[ContentRoute.ContentPageKey], Is.EqualTo(root.ID));
			Assert.That(r.DataTokens[ContentRoute.ContentPageKey], Is.EqualTo(root));
		}

		[Test]
		public void RoutingToPart_PassesController_OfThePart()
		{
			var part = CreateOneItem<TestItem>(10, "whatever", root);
			var r = RoutePath("/TestItem/?part=10");

			Assert.That(r.Values[ContentRoute.ControllerKey], Is.EqualTo("TestItem"));
		}

		[Test]
		public void RoutingToPart_PassesAction_DefinedByQueryString()
		{
			var part = CreateOneItem<TestItem>(10, "whatever", root);
			RoutePath("/TestItem/WithModel/?part=10");

			var r = routes.GetRouteData(httpContext);
			Assert.That(r.Values[ContentRoute.ActionKey], Is.EqualTo("WithModel"));
		}

		[Test]
		public void RoutingToPart_PassesPage_AsContentPage()
		{
			var part = CreateOneItem<TestItem>(10, "whatever", root);
			var r = RoutePath("/TestItem/?part=10&page=" + search.ID);

			Assert.That(r.Values[ContentRoute.ContentPageKey], Is.EqualTo(search.ID));
			Assert.That(r.DataTokens[ContentRoute.ContentPageKey], Is.EqualTo(search));
		}

		[Test]
		public void RoutingToPart_PassesAssociatedPart_AsContentItem()
		{
			RoutePath("/");

			var part = CreateOneItem<TestItem>(10, "whatever", root);
			var vpd = routes.GetVirtualPath(requestContext, new RouteValueDictionary(new { item = part }));
			RoutePath(vpd.VirtualPath);

			var r = routes.GetRouteData(httpContext);
			Assert.That(r.Values[ContentRoute.ContentPartKey], Is.EqualTo(part.ID));
			Assert.That(r.DataTokens[ContentRoute.ContentPartKey], Is.EqualTo(part));
		}

		[Test, Ignore("TODO (maybe)")]
		public void CanRoute_ToPartVirtualPath_Passes_AssociatedPage()
		{
			RoutePath("/");

			var part = CreateOneItem<TestItem>(10, "whatever", root);
			var vpd = routes.GetVirtualPath(requestContext, new RouteValueDictionary(new { item = part }));
			RoutePath(vpd.VirtualPath);

			var r = routes.GetRouteData(httpContext);
			Assert.That(r.Values[ContentRoute.ContentPageKey], Is.EqualTo(root.ID.ToString()));
			Assert.That(r.DataTokens[ContentRoute.ContentPageKey], Is.EqualTo(root));
		}

		/*** GetVirtualPath parts ***/

		[Test]
		public void GetVirtualPath_ToPartDefaultAction()
		{
			var part = CreateOneItem<TestItem>(10, "whatever", about);
			RoutePath("/about/");

			var vpd = route.GetVirtualPath(requestContext, new RouteValueDictionary(new { item = part }));

			Assert.That(vpd.VirtualPath, Is.EqualTo("TestItem?page=2&part=10"));
		}

		[Test]
		public void GetVirtualPath_ToPartDefaultAction_ViaPart()
		{
			var part = CreateOneItem<TestItem>(10, "whatever", about);
			RoutePath("/about/");

			var vpd = route.GetVirtualPath(requestContext, new RouteValueDictionary(new { part = part }));

			Assert.That(vpd.VirtualPath, Is.EqualTo("TestItem?page=2&part=10"));
		}

		[Test]
		public void GetVirtualPath_ToPart_OtherActionAction()
		{
			var part = CreateOneItem<TestItem>(10, "whatever", about);
			RoutePath("/about/");

			var vpd = route.GetVirtualPath(requestContext, new RouteValueDictionary(new { item = part, action = "doit" }));

			Assert.That(vpd.VirtualPath, Is.EqualTo("TestItem/doit?page=2&part=10"));
		}

		[Test]
		public void CanCreate_ActionLink_ToPart()
		{
			var rc = CreateRouteContext(CreateOneItem<TestItem>(10, "whatever", root));
			var helper = new HtmlHelper(CreateViewContext(rc), new ViewPage(), routes);

			var html = helper.ActionLink("Hello"/*text*/, "Submit"/*action*/, new { q = "something", controller = "TestItem" });

			Assert.That(html.ToString(), Is.EqualTo("<a href=\"/TestItem/Submit?q=something&amp;page=1&amp;part=10\">Hello</a>"));
		}



		ViewContext CreateViewContext(RequestContext rc)
		{
			return new ViewContext { RequestContext = rc, HttpContext = httpContext, RouteData = rc.RouteData };
		}

		RouteData RoutePath(Url url)
		{
			httpContext.request.appRelativeCurrentExecutionFilePath = "~" + url.Path;
			httpContext.request.rawUrl = url;
			httpContext.request.query = url.GetQueries().ToNameValueCollection();
			foreach (var kvp in url.GetQueries())
				httpContext.request.query[kvp.Key] = kvp.Value;
			var data = route.GetRouteData(httpContext);
			requestContext = new RequestContext(httpContext, data);
			return data;
		}

		private RequestContext CreateRouteContext(ContentItem item)
		{
			RoutePath(item.Url);

			var ctx = new RequestContext(httpContext, new RouteData());
			ctx.RouteData.DataTokens[ContentRoute.ContentItemKey] = item;
			ctx.RouteData.Values[ContentRoute.ControllerKey] = controllerMapper.GetControllerName(item.GetType());
			return ctx;
		}
	}

	static class UrlExtensions
	{
		public static NameValueCollection ToNameValueCollection(this IDictionary<string, string> dictionary)
		{
			NameValueCollection nvc = new NameValueCollection();
			foreach (var kvp in dictionary)
				nvc[kvp.Key] = kvp.Value;
			return nvc;
		}
	}
}
