using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Web.Mvc;
using System.Web.Mvc;
using System.Web.Routing;
using System.IO;
using N2.Extensions.Tests.Fakes;
using Rhino.Mocks;
using N2.Engine;
using N2.Web.Mvc.Html;
using N2.Web.Rendering;

namespace N2.Extensions.Tests.Mvc
{
    public class MvcTestUtilities
    {
        public static ViewPage<T> CreateViewPage<T>(T model)
            where T : ContentItem
        {
            var page = new ViewPage<T>();
            page.ViewData = new ViewDataDictionary<T>(model);
            page.ViewContext = new ViewContext(new ControllerContext(), new WebFormView("~/page.aspx"), page.ViewData, new TempDataDictionary(), new StringWriter());
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
				.Return(new DisplayableRendererSelector(new IDisplayableRenderer[] { new LiteralDisplayableRenderer(), new FallbackDisplayableRenderer() }))
				.Repeat.Any();
			return engine;
		}

        private class StubController : Controller
        {
        }
    }
}
