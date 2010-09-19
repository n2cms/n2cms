using System.Collections.Specialized;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
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
	public abstract class ContentRouteTests : N2.Tests.ItemPersistenceMockingBase
	{
		// /					-> RegularPage
		// /about/				-> AboutUsSectionPage
		// /about/executives/	-> ExecutiveTeamPage
		// /search/				-> SearchPage

		#region SetUp

		protected IEngine engine;
		protected FakeHttpContext httpContext;
		protected RequestContext requestContext = null;
		protected RouteCollection routes;
		protected IControllerMapper controllerMapper;

		protected RegularPage root;
		protected AboutUsSectionPage about;
		protected ExecutiveTeamPage executives;
		protected SearchPage search;
		protected ContentRoute route;
		protected UrlHelper urlHelper;
		protected HtmlHelper htmlHelper;

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

		#region RequestingUrl
		protected RouteData RequestingUrl(Url url)
		{
			httpContext.request.appRelativeCurrentExecutionFilePath = "~" + url.Path;
			httpContext.request.rawUrl = url;

			NameValueCollection nvc = new NameValueCollection();
			foreach (var kvp in url.GetQueries())
				nvc[kvp.Key] = kvp.Value;
			httpContext.request.query = nvc;

			foreach (var kvp in url.GetQueries())
				httpContext.request.query[kvp.Key] = kvp.Value;

			var data = route.GetRouteData(httpContext);
			requestContext = new RequestContext(httpContext, data);
			urlHelper = new UrlHelper(requestContext, routes);
			htmlHelper = new HtmlHelper(new ViewContext { RequestContext = requestContext, HttpContext = httpContext, RouteData = requestContext.RouteData, Writer = httpContext.Response.Output }, new ViewPage(), routes);
			return data;
		}
		#endregion
	}


	//[TestFixture]
	//public class ContentRouteTests_GetRouteData : ContentRouteTests
	//{
	//    [Test]
	//    public void CanFindController_ForStartPage()
	//    {
	//        var data = RequestingUrl("/");

	//        Assert.That(data.CurrentItem(), Is.EqualTo(root));
	//        Assert.That(data.CurrentPage(), Is.EqualTo(root));
	//        Assert.That(data.Values["controller"], Is.EqualTo("Regular"));
	//        Assert.That(data.Values["action"], Is.EqualTo("index"));
	//    }

	//    [Test]
	//    public void CanFindController_ForStartPage_default_aspx()
	//    {
	//        var data = RequestingUrl("/default.aspx");

	//        Assert.That(data.CurrentItem(), Is.EqualTo(root));
	//        Assert.That(data.CurrentPage(), Is.EqualTo(root));
	//        Assert.That(data.Values["controller"], Is.EqualTo("Regular"));
	//        Assert.That(data.Values["action"], Is.EqualTo("index"));
	//    }

	//    [Test]
	//    public void CanFindController_ForContentPage()
	//    {
	//        var data = RequestingUrl("/about/");

	//        Assert.That(data.CurrentItem(), Is.EqualTo(about));
	//        Assert.That(data.CurrentPage(), Is.EqualTo(about));
	//        Assert.That(data.Values["controller"], Is.EqualTo("AboutUsSectionPage"));
	//        Assert.That(data.Values["action"], Is.EqualTo("index"));
	//    }

	//    [Test]
	//    public void CanFindController_ForContentPage_NoSlash()
	//    {
	//        var data = RequestingUrl("/about");

	//        Assert.That(data.CurrentItem(), Is.EqualTo(about));
	//        Assert.That(data.CurrentPage(), Is.EqualTo(about));
	//        Assert.That(data.Values["controller"], Is.EqualTo("AboutUsSectionPage"));
	//        Assert.That(data.Values["action"], Is.EqualTo("index"));
	//    }

	//    [Test]
	//    public void RoutesOnlyToPage_WhenPart_IsPassedAsItem_InQuery()
	//    {
	//        var part = CreateOneItem<TestItem>(10, "whatever", root);
	//        var data = RequestingUrl("/about/?item=10");

	//        Assert.That(data.CurrentItem(), Is.EqualTo(about));
	//        Assert.That(data.CurrentPage(), Is.EqualTo(about));
	//        Assert.That(data.Values["controller"], Is.EqualTo("AboutUsSectionPage"));
	//        Assert.That(data.Values["action"], Is.EqualTo("index"));
	//    }

	//    [Test]
	//    public void CanFindController_ForExtendingType()
	//    {
	//        RequestingUrl("/about/executives/");

	//        var data = route.GetRouteData(httpContext);

	//        Assert.That(data.CurrentItem(), Is.EqualTo(executives));
	//        Assert.That(data.CurrentPage(), Is.EqualTo(executives));
	//        Assert.That(data.Values["controller"], Is.EqualTo("ExecutiveTeam"));
	//        Assert.That(data.Values["action"], Is.EqualTo("index"));
	//    }

	//    [Test]
	//    public void CanGet_ActionName_FromUrl()
	//    {
	//        RequestingUrl("/about/submit");

	//        var routeData = route.GetRouteData(httpContext);

	//        Assert.That(routeData, Is.Not.Null);
	//        Assert.That(routeData.Values["controller"], Is.EqualTo("AboutUsSectionPage"));
	//        Assert.That(routeData.Values["action"], Is.EqualTo("submit"));
	//    }

	//    [Test]
	//    public void CanRoute_ToPage_ViaController()
	//    {
	//        var part = CreateOneItem<TestItem>(10, "whatever", root);
	//        RequestingUrl("/Regular/?page=1");

	//        var r = routes.GetRouteData(httpContext);

	//        Assert.That(r.CurrentItem(), Is.EqualTo(root));
	//        Assert.That(r.CurrentPage(), Is.EqualTo(root));
	//        Assert.That(r.Values[ContentRoute.ControllerKey], Is.EqualTo("Regular"));
	//        Assert.That(r.Values[ContentRoute.ActionKey], Is.EqualTo("index"));
	//    }

	//    [Test, Ignore("TODO")]
	//    public void CanGet_IdParameter_FromUrl()
	//    {
	//        RequestingUrl("/about/submit/123");

	//        var routeData = route.GetRouteData(httpContext);

	//        Assert.That(routeData, Is.Not.Null);
	//        Assert.That(routeData.Values["controller"], Is.EqualTo("AboutUsSectionPage"));
	//        Assert.That(routeData.Values["action"], Is.EqualTo("submit"));
	//        Assert.That(routeData.Values["id"], Is.EqualTo("123"));
	//    }

	//}

	//[TestFixture]
	//public class ContentRouteTests_GetVirtualPath : ContentRouteTests
	//{
	//    [Test]
	//    public void VirtualPath_IsNull_ForOtherRequestedController()
	//    {
	//        RequestingUrl("/about/");

	//        var virtualPath = route.GetVirtualPath(requestContext, new RouteValueDictionary(new { controller = "Mvc" }));

	//        Assert.That(virtualPath, Is.Null);
	//    }

	//    [Test]
	//    public void AppendsActionName_ToContentUrl()
	//    {
	//        RequestingUrl("/about/");

	//        var result = urlHelper.Action("hello");

	//        Assert.That(result, Is.EqualTo("/about/hello"));
	//    }

	//    [Test]
	//    public void DoesntRoute_ControllerOnly()
	//    {
	//        RequestingUrl("/about/");

	//        var result = urlHelper.Action("hello", "other");

	//        Assert.That(result, Is.Null);
	//    }

	//    [Test]
	//    public void CanCreate_ActionUrl_ToPage()
	//    {
	//        RequestingUrl("/about/");

	//        var result = urlHelper.Action("index", new { item = executives });

	//        Assert.That(result, Is.EqualTo("/about/executives"));
	//    }

	//    [Test]
	//    public void GetVirtualPath_ToSelf()
	//    {
	//        RequestingUrl("/search/");

	//        var vpd = route.GetVirtualPath(requestContext, new RouteValueDictionary());

	//        Assert.That(vpd.VirtualPath, Is.EqualTo("search"));
	//    }

	//    [Test]
	//    public void GetVirtualPath_ToOtherAction_OnSelf()
	//    {
	//        RequestingUrl("/search/");

	//        var vpd = route.GetVirtualPath(requestContext, new RouteValueDictionary(new { action = "find" }));

	//        Assert.That(vpd.VirtualPath, Is.EqualTo("search/find"));
	//    }

	//    [Test]
	//    public void GetVirtualPath_ToOtherAction_OnSelf_WithParameter()
	//    {
	//        RequestingUrl("/search/");

	//        var vpd = route.GetVirtualPath(requestContext, new RouteValueDictionary(new { action = "find", q = "query" }));

	//        Assert.That(vpd.VirtualPath, Is.EqualTo("search/find?q=query"));
	//    }

	//    [Test]
	//    public void GetVirtualPathTo_OtherContentItem()
	//    {
	//        RequestingUrl("/search/");

	//        var vpd = route.GetVirtualPath(requestContext, new RouteValueDictionary(new { item = executives }));

	//        Assert.That(vpd.VirtualPath, Is.EqualTo("about/executives"));
	//    }

	//    [Test]
	//    public void GetVirtualPathTo_OtherContentItem_ViaPage_IsNoLongerSupported()
	//    {
	//        RequestingUrl("/search/");

	//        var vpd = route.GetVirtualPath(requestContext, new RouteValueDictionary(new { page = executives }));

	//        Assert.That(vpd.VirtualPath, Is.Not.EqualTo("about/executives"));
	//    }

	//    [Test]
	//    public void GetVirtualPathTo_SameController_IsNotNull()
	//    {
	//        RequestingUrl("/about/");

	//        var vpd = route.GetVirtualPath(requestContext, new RouteValueDictionary(new { controller = "aboutussectionpage" }));

	//        Assert.That(vpd, Is.Not.Null);
	//        Assert.That(vpd.VirtualPath, Is.EqualTo("about"));
	//    }

	//    [Test]
	//    public void GetVirtualPathTo_SameController_AddsAction()
	//    {
	//        RequestingUrl("/about/");

	//        var vpd = route.GetVirtualPath(requestContext, new RouteValueDictionary(new { controller = "aboutussectionpage", action = "more" }));

	//        Assert.That(vpd, Is.Not.Null);
	//        Assert.That(vpd.VirtualPath, Is.EqualTo("about/more"));
	//    }

	//    [Test]
	//    public void GetVirtualPathTo_OtherController_IsNull()
	//    {
	//        RequestingUrl("/about/");

	//        var vpd = route.GetVirtualPath(requestContext, new RouteValueDictionary(new { controller = "hello", action = "index" }));

	//        Assert.That(vpd, Is.Null);
	//    }

	//    [Test]
	//    public void GetVirtualPath_ToAction_OnOtherContentItem()
	//    {
	//        RequestingUrl("/about/executives/");

	//        var vpd = route.GetVirtualPath(requestContext, new RouteValueDictionary(new { item = search, action = "find" }));

	//        Assert.That(vpd.VirtualPath, Is.EqualTo("search/find"));
	//    }

	//    [Test]
	//    public void GetVirtualPath_ToAction_OnOtherContentItem_WithParameters()
	//    {
	//        RequestingUrl("/about/executives/");

	//        var vpd = route.GetVirtualPath(requestContext, new RouteValueDictionary(new { item = search, action = "find", q = "what", x = "y" }));

	//        Assert.That(vpd.VirtualPath, Is.EqualTo("search/find?q=what&x=y"));
	//    }

	//    [Test]
	//    public void CanGenerate_DefaultRouteUrl()
	//    {
	//        RequestingUrl("/search/");

	//        string url = urlHelper.RouteUrl(null, new { controller = "Search" }, null);

	//        Assert.That(url, Is.EqualTo("/search"));
	//    }

	//    [Test]
	//    public void CanGenerateLink()
	//    {
	//        RequestingUrl("/search/");

	//        string html = HtmlHelper.GenerateLink(requestContext, routes, "Hello", null, "find", "Search", new RouteValueDictionary(new { q = "hello" }), null);

	//        Assert.That(html, Is.EqualTo("<a href=\"/search/find?q=hello\">Hello</a>"));
	//    }

	//    [Test]
	//    public void CanCreate_ActionLink()
	//    {
	//        RequestingUrl("/search/");

	//        var html = htmlHelper.ActionLink("Hello", "find", new { q = "something", controller = "Search" });

	//        Assert.That(html.ToString(), Is.EqualTo("<a href=\"/search/find?q=something\">Hello</a>"));
	//    }

	//    [Test]
	//    public void CanCreate_UrlFromExpression()
	//    {
	//        RequestingUrl("/search/");

	//        string html = htmlHelper.BuildUrlFromExpression<SearchController>(s => s.Find("hello"));

	//        Assert.That(html, Is.EqualTo("/search/Find?q=hello"));
	//    }
	//}

	//[TestFixture]
	//public class ContentRouteTests_GetRouteData_Parts : ContentRouteTests
	//{
	//    [Test]
	//    public void CanRoute_ToPart()
	//    {
	//        var part = CreateOneItem<TestItem>(10, "whatever", root);
	//        RequestingUrl("/TestItem/?part=10");

	//        var r = routes.GetRouteData(httpContext);

	//        Assert.That(r.CurrentItem(), Is.EqualTo(part));
	//        Assert.That(r.CurrentPage(), Is.EqualTo(root));
	//        Assert.That(r.Values[ContentRoute.ControllerKey], Is.EqualTo("TestItem"));
	//        Assert.That(r.Values[ContentRoute.ActionKey], Is.EqualTo("Index"));
	//    }

	//    [Test]
	//    public void CanRoute_ToPart_WithPage()
	//    {
	//        var part = CreateOneItem<TestItem>(10, "whatever", root);
	//        RequestingUrl("/TestItem/?page=1&part=10");

	//        var r = routes.GetRouteData(httpContext);

	//        Assert.That(r.CurrentItem(), Is.EqualTo(part));
	//        Assert.That(r.CurrentPage(), Is.EqualTo(root));
	//        Assert.That(r.Values[ContentRoute.ControllerKey], Is.EqualTo("TestItem"));
	//        Assert.That(r.Values[ContentRoute.ActionKey], Is.EqualTo("Index"));
	//    }

	//    [Test]
	//    public void CanRoute_ToPart_Action()
	//    {
	//        var part = CreateOneItem<TestItem>(10, "whatever", root);
	//        RequestingUrl("/TestItem/Submit/?part=10");

	//        var r = routes.GetRouteData(httpContext);

	//        Assert.That(r.CurrentItem(), Is.EqualTo(part));
	//        Assert.That(r.Values[ContentRoute.ControllerKey], Is.EqualTo("TestItem"));
	//        Assert.That(r.Values[ContentRoute.ActionKey], Is.EqualTo("Submit"));
	//    }

	//    [Test]
	//    public void CanRoute_ToPart_Action_WithPage()
	//    {
	//        var part = CreateOneItem<TestItem>(10, "whatever", root);
	//        RequestingUrl("/TestItem/Submit/?page=1&part=10");

	//        var r = routes.GetRouteData(httpContext);

	//        Assert.That(r.CurrentItem(), Is.EqualTo(part));
	//        Assert.That(r.CurrentPage(), Is.EqualTo(root));
	//        Assert.That(r.Values[ContentRoute.ControllerKey], Is.EqualTo("TestItem"));
	//        Assert.That(r.Values[ContentRoute.ActionKey], Is.EqualTo("Submit"));
	//    }

	//    [Test]
	//    public void CanRoute_ToPart_Action_WithPageOtherThanParent()
	//    {
	//        var part = CreateOneItem<TestItem>(10, "whatever", root);
	//        RequestingUrl("/TestItem/Submit/?part=10&page=" + about.ID);

	//        var r = routes.GetRouteData(httpContext);

	//        Assert.That(r.CurrentItem(), Is.EqualTo(part));
	//        Assert.That(r.CurrentPage(), Is.EqualTo(about));
	//        Assert.That(r.Values[ContentRoute.ControllerKey], Is.EqualTo("TestItem"));
	//        Assert.That(r.Values[ContentRoute.ActionKey], Is.EqualTo("Submit"));
	//    }

	//    [Test]
	//    public void CanRoute_ToPart_PageIsClosestParent()
	//    {
	//        var part = CreateOneItem<TestItem>(10, "whatever", root);
	//        RequestingUrl("/TestItem/Submit/?part=10");

	//        var r = routes.GetRouteData(httpContext);

	//        Assert.That(r.CurrentItem(), Is.EqualTo(part));
	//        Assert.That(r.CurrentPage(), Is.EqualTo(root));
	//        Assert.That(r.Values[ContentRoute.ControllerKey], Is.EqualTo("TestItem"));
	//        Assert.That(r.Values[ContentRoute.ActionKey], Is.EqualTo("Submit"));
	//    }

	//    [Test]
	//    public void RoutingToPart_ViaItsVirtualPath_PassesController_OfThePart()
	//    {
	//        RequestingUrl("/");

	//        var part = CreateOneItem<TestItem>(10, "whatever", root);
	//        var vpd = routes.GetVirtualPath(requestContext, new RouteValueDictionary(new { item = part }));
	//        RequestingUrl(vpd.VirtualPath);

	//        var r = routes.GetRouteData(httpContext);
	//        Assert.That(r.Values[ContentRoute.ControllerKey], Is.EqualTo("TestItem"));
	//    }

	//    [Test]
	//    public void RoutingToPart_ViaItsVirtualPath_PassesAssociatedPart_AsContentPart()
	//    {
	//        var part = CreateOneItem<TestItem>(10, "whatever", root);

	//        RequestingUrl("/");

	//        var vpd = routes.GetVirtualPath(requestContext, new RouteValueDictionary(new { item = part }));
	//        RequestingUrl(vpd.VirtualPath);
	//        var r = routes.GetRouteData(httpContext);

	//        Assert.That(r.CurrentItem(), Is.EqualTo(part));
	//        Assert.That(r.CurrentPage(), Is.EqualTo(root));
	//    }

	//    [Test]
	//    public void RoutingToPart_PassesPart_AsContentPart()
	//    {
	//        var part = CreateOneItem<TestItem>(10, "whatever", root);
	//        var r = RequestingUrl("/TestItem/?part=10");

	//        Assert.That(r.Values[ContentRoute.ContentPartKey], Is.EqualTo(part.ID));
	//        Assert.That(r.DataTokens[ContentRoute.ContentPartKey], Is.EqualTo(part));
	//    }

	//    [Test]
	//    public void RoutingToPart_PassesAssociatedPage_AsContentPage()
	//    {
	//        var part = CreateOneItem<TestItem>(10, "whatever", root);
	//        var r = RequestingUrl("/TestItem/?part=10");

	//        Assert.That(r.Values[ContentRoute.ContentPageKey], Is.EqualTo(root.ID));
	//        Assert.That(r.DataTokens[ContentRoute.ContentPageKey], Is.EqualTo(root));
	//    }

	//    [Test]
	//    public void RoutingToPart_PassesController_OfThePart()
	//    {
	//        var part = CreateOneItem<TestItem>(10, "whatever", root);
	//        var r = RequestingUrl("/TestItem/?part=10");

	//        Assert.That(r.Values[ContentRoute.ControllerKey], Is.EqualTo("TestItem"));
	//    }

	//    [Test]
	//    public void RoutingToPart_PassesAction_DefinedByQueryString()
	//    {
	//        var part = CreateOneItem<TestItem>(10, "whatever", root);
	//        RequestingUrl("/TestItem/WithModel/?part=10");

	//        var r = routes.GetRouteData(httpContext);
	//        Assert.That(r.Values[ContentRoute.ActionKey], Is.EqualTo("WithModel"));
	//    }

	//    [Test]
	//    public void RoutingToPart_PassesPage_AsContentPage()
	//    {
	//        var part = CreateOneItem<TestItem>(10, "whatever", root);
	//        var r = RequestingUrl("/TestItem/?part=10&page=" + search.ID);

	//        Assert.That(r.Values[ContentRoute.ContentPageKey], Is.EqualTo(search.ID));
	//        Assert.That(r.DataTokens[ContentRoute.ContentPageKey], Is.EqualTo(search));
	//    }

	//    [Test]
	//    public void RoutingToPart_PassesAssociatedPart_AsContentItem()
	//    {
	//        RequestingUrl("/");

	//        var part = CreateOneItem<TestItem>(10, "whatever", root);
	//        var vpd = routes.GetVirtualPath(requestContext, new RouteValueDictionary(new { item = part }));
	//        RequestingUrl(vpd.VirtualPath);

	//        var r = routes.GetRouteData(httpContext);
	//        Assert.That(r.Values[ContentRoute.ContentPartKey], Is.EqualTo(part.ID));
	//        Assert.That(r.DataTokens[ContentRoute.ContentPartKey], Is.EqualTo(part));
	//    }

	//    [Test, Ignore("TODO (maybe)")]
	//    public void CanRoute_ToPartVirtualPath_Passes_AssociatedPage()
	//    {
	//        RequestingUrl("/");

	//        var part = CreateOneItem<TestItem>(10, "whatever", root);
	//        var vpd = routes.GetVirtualPath(requestContext, new RouteValueDictionary(new { item = part }));
	//        RequestingUrl(vpd.VirtualPath);

	//        var r = routes.GetRouteData(httpContext);
	//        Assert.That(r.Values[ContentRoute.ContentPageKey], Is.EqualTo(root.ID.ToString()));
	//        Assert.That(r.DataTokens[ContentRoute.ContentPageKey], Is.EqualTo(root));
	//    }
	//}

	//[TestFixture]
	//public class ContentRouteTests_GetVirtualPath_Parts : ContentRouteTests
	//{
	//    [Test]
	//    public void GetVirtualPath_ToPartDefaultAction()
	//    {
	//        var part = CreateOneItem<TestItem>(10, "whatever", about);
	//        RequestingUrl("/about/");

	//        var vpd = route.GetVirtualPath(requestContext, new RouteValueDictionary(new { item = part }));

	//        Assert.That(vpd.VirtualPath, Is.EqualTo("TestItem?page=2&part=10"));
	//    }

	//    [Test]
	//    public void GetVirtualPath_ToPartDefaultAction_ViaPart_IsNotSupported()
	//    {
	//        var part = CreateOneItem<TestItem>(10, "whatever", about);
	//        RequestingUrl("/about/");

	//        var vpd = route.GetVirtualPath(requestContext, new RouteValueDictionary(new { part = part }));

	//        Assert.That(vpd.VirtualPath, Is.Not.EqualTo("TestItem?page=2&part=10"));
	//    }

	//    [Test]
	//    public void GetVirtualPath_ToPart_OtherActionAction()
	//    {
	//        var part = CreateOneItem<TestItem>(10, "whatever", about);
	//        RequestingUrl("/about/");

	//        var vpd = route.GetVirtualPath(requestContext, new RouteValueDictionary(new { item = part, action = "doit" }));

	//        Assert.That(vpd.VirtualPath, Is.EqualTo("TestItem/doit?page=2&part=10"));
	//    }

	//    [Test]
	//    public void CanCreate_ActionLink_ToPart()
	//    {
	//        var item = CreateOneItem<TestItem>(10, "whatever", root);
	//        RequestingUrl("/?part=10");

	//        var html = htmlHelper.ActionLink(/*text*/"Hello", /*action*/"Submit", new { q = "something", controller = "TestItem" });

	//        Assert.That(html.ToString(), Is.EqualTo("<a href=\"/TestItem/Submit?q=something&amp;page=1&amp;part=10\">Hello</a>"));
	//    }
	//}

	//[TestFixture]
	//public class ContentRouteTests_RenderAction : ContentRouteTests
	//{
	//    [Test]
	//    public void CanRender_NonContentController()
	//    {
	//        routes.MapRoute("test", "{controller}/{action}");
	//        RequestingUrl("/");

	//        htmlHelper.RenderAction("top", "navigation");

	//        string renderedValues = httpContext.Response.Output.ToString();
	//        Assert.That(renderedValues.Contains("controller=navigation"));
	//        Assert.That(renderedValues.Contains("action=top"));
	//    }
	//}

}
