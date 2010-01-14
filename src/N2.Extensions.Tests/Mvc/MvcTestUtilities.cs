using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Web.Mvc;
using System.Web.Mvc;
using System.Web.Routing;

namespace N2.Extensions.Tests.Mvc
{
    public class MvcTestUtilities
    {
        public static ViewPage<T> CreateViewPage<T>(T model)
            where T : ContentItem
        {
            var page = new ViewPage<T>();
            page.ViewData = new ViewDataDictionary<T>(model);
            page.ViewContext = new ViewContext(new ControllerContext(), new WebFormView("~/page.aspx"), page.ViewData, new TempDataDictionary());
			page.ViewContext.RouteData.DataTokens[ContentRoute.ContentItemKey] = model;
            return page;
        }

        public static ContentViewPage<TModel, TItem> CreateContentViewPage<TModel, TItem>(TModel model, TItem item)
            where TModel : class
            where TItem : ContentItem
        {
            var page = new ContentViewPage<TModel, TItem>();
            page.ViewData = new ViewDataDictionary<TModel>(model);
            var controllerContext = new ControllerContext { 
                RouteData = new RouteData(new Route("anything", new MvcRouteHandler()), new MvcRouteHandler()),
                Controller = new StubController()
            };
			controllerContext.RequestContext.RouteData.DataTokens[ContentRoute.ContentItemKey] = item;
            controllerContext.Controller.ControllerContext = controllerContext;

            page.ViewContext = new ViewContext(controllerContext, new WebFormView("~/page.aspx"), page.ViewData, new TempDataDictionary());
            return page;
        }

        private class StubController : Controller
        {
        }
    }
}
