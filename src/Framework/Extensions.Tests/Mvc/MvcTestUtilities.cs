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
using N2.Web;

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
            var cc = new ControllerContext { HttpContext = ctx };
            page.ViewContext = new ViewContext(cc, new WebFormView(cc, "~/page.aspx"), page.ViewData, new TempDataDictionary(), new StringWriter()) { HttpContext = ctx };
            page.ViewContext.RouteData.ApplyCurrentPath(new Web.PathData(model));
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
            controllerContext.RequestContext.RouteData.ApplyCurrentPath(new Web.PathData(item));
            controllerContext.Controller.ControllerContext = controllerContext;

            page.ViewContext = new ViewContext(controllerContext, new WebFormView(controllerContext, "~/page.aspx"), page.ViewData, new TempDataDictionary(), new StringWriter())
            {
                HttpContext = controllerContext.HttpContext
            };
            return page;
        }

        private static IEngine StubEngine()
        {
            var engine = new FakeEngine();
            engine.AddComponentInstance<ITemplateRenderer>(new TemplateRenderer(MockRepository.GenerateStub<IControllerMapper>()));
            engine.AddComponentInstance<DisplayableRendererSelector>(new DisplayableRendererSelector(new IDisplayableRenderer[] { new WritingDisplayableRenderer(), new FallbackDisplayableRenderer() }));
            engine.AddComponentInstance<ISecurityManager>(new FakeSecurityManager());
            engine.AddComponentInstance<IWebContext>(new ThreadContext());
            return engine;
        }

        private class StubController : Controller
        {
        }
    }
}
