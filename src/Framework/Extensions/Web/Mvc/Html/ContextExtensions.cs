using System.Web.Mvc;

namespace N2.Web.Mvc.Html
{
    internal static class ContextExtensions
    {
        internal static HtmlHelper<TItem> CreateContentItemHelper<TItem>(this ViewContext viewContext)
            where TItem : ContentItem
        {
            var contentDictionary = new ViewDataDictionary<TItem>(viewContext.RouteData.CurrentItem() as TItem);
            var controllerContext = viewContext.Controller.ControllerContext;
            var contentContext = new ViewContext(controllerContext, viewContext.View, contentDictionary, viewContext.TempData, viewContext.Writer);
            var content = new HtmlHelper<TItem>(contentContext, new ItemViewDataContainer { ViewData = contentDictionary });
            return content;
        }

        class ItemViewDataContainer : IViewDataContainer
        {
            public ViewDataDictionary ViewData { get; set; }
        }
    }
}
