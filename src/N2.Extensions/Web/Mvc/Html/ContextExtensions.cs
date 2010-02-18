using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace N2.Web.Mvc.Html
{
    internal static class ContextExtensions
    {
        internal static HtmlHelper<TItem> CreateContentItemHelper<TItem>(this ViewContext viewContext)
            where TItem : ContentItem
        {
            var contentDictionary = new ViewDataDictionary<TItem>(viewContext.CurrentItem<TItem>());
            var controllerContext = viewContext.Controller.ControllerContext;
            var contentContext = new ViewContext(controllerContext, viewContext.View, contentDictionary, viewContext.TempData, viewContext.HttpContext.Response.Output);
            var content = new HtmlHelper<TItem>(contentContext, new ItemViewDataContainer { ViewData = contentDictionary });
            return content;
        }

        class ItemViewDataContainer : IViewDataContainer
        {
            public ViewDataDictionary ViewData { get; set; }
        }
    }
}
