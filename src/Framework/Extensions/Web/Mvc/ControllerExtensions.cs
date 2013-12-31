using System.Web.Mvc;

namespace N2.Web.Mvc
{
    public static class ControllerExtensions
    {
        public static ViewResult View(this Controller controller, object model, ContentItem item)
        {
            return controller.View(null, model, item);
        }
        public static ViewResult View(this Controller controller, string viewName, object model, ContentItem item)
        {
            if (model != null)
            {
                controller.ViewData.Model = model;
            }
            controller.ViewData[ContentRoute.ContentItemKey] = item;

            ViewResult result = new ViewResult();
            result.ViewName = viewName;
            result.ViewData = controller.ViewData;
            result.TempData = controller.TempData;
            return result;
        }
    }
}
