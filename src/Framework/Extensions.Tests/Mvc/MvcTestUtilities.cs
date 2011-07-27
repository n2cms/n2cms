using System.IO;
using System.Web.Mvc;
using System.Web.Routing;
using N2.Engine;
using N2.Tests.Fakes;
using N2.Web.Mvc;
using N2.Web.Mvc.Html;
using N2.Web.Rendering;
using Rhino.Mocks;
using N2.Security;

namespace N2.Extensions.Tests.Mvc
{
    public class MvcTestUtilities
    {
        public static ViewPage<T> CreateViewPage<T>(T model)
            where T : ContentItem
        {
            var page = new ViewPage<T>();
            page.ViewData = new ViewDataDictionary<T>(model);
			var ctx = new FakeHttpContext();
			page.ViewContext = new ViewContext(new ControllerContext { HttpContext = ctx }, new WebFormView("~/page.aspx"), page.ViewData, new TempDataDictionary(), new StringWriter()) { HttpContext = ctx };
			page.ViewContext.RouteData.DataTokens[ContentRoute.ContentItemKey] = model;
			page.ViewContext.RouteData.DataTokens[ContentRoute.ContentEngineKey] = StubEngine();
			return page;
        }

        public static ContentViewPage<TModel, TItem> CreateContentViewPage<TModel, TItem>(TModel model, TItem item)
            where TModel : class
            where TItem : ContentItem
        {
            var page = new ContentViewPage<TModel, TItem>();
            page.ViewData = new ViewDataDictionary<TModel>(model);
			var rd = new RouteData(new Route("anything", new MvcRouteHandler()), new MvcRouteHandler());
			rd.DataTokens[ContentRoute.ContentEngineKey] = StubEngine();
			var controllerContext = new ControllerContext
			{ 
                RouteData = rd,
                Controller = new StubController(),
				HttpContext = new FakeHttpContext("/")
            };
			controllerContext.RequestContext.RouteData.DataTokens[ContentRoute.ContentItemKey] = item;
            controllerContext.Controller.ControllerContext = controllerContext;

			page.ViewContext = new ViewContext(controllerContext, new WebFormView("~/page.aspx"), page.ViewData, new TempDataDictionary(), new StringWriter())
			{
				HttpContext = controllerContext.HttpContext
			};
            return page;
        }

		private static IEngine StubEngine()
		{
			var engine = MockRepository.GenerateStub<IEngine>();
			engine.Expect(e => e.Resolve<ITemplateRenderer>())
				.Return(new TemplateRenderer(MockRepository.GenerateStub<IControllerMapper>()))
				.Repeat.Any();
			engine.Expect(e => e.Resolve<DisplayableRendererSelector>())
				.Return(new DisplayableRendererSelector(new IDisplayableRenderer[] { new WritingDisplayableRenderer(), new FallbackDisplayableRenderer() }))
				.Repeat.Any();
			engine.Expect(e => e.SecurityManager)
				.Return(MockRepository.GenerateStub<ISecurityManager>())
				.Repeat.Any();
			return engine;
		}

        private class StubController : Controller
        {
        }
    }
}
