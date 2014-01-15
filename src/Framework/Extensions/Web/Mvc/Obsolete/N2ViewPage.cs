using System;
using System.Collections;
using System.Web;
using System.Web.Mvc;
using N2.Web.UI;

namespace N2.Web.Mvc
{
    /// <summary>
    /// A ViewPage implementation that allows N2 Display helpers to be used
    /// 
    /// The Model must be a ContentItem
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    [Obsolete("Use System.Web.Mvc.ViewPage<>")]
    public class N2ViewPage<TItem> : ViewPage<TItem>, IItemContainer
        where TItem : ContentItem
    {
        #region IItemContainer Members

        public TItem CurrentItem
        {
            get { return Html.ViewData.Model; }
            set { Html.ViewData.Model = value; }
        }
        ContentItem IItemContainer.CurrentItem
        {
            get { return CurrentItem; }
        }

        #endregion

        public HtmlHelper<TItem> Content { get { return Html; } }

        public override void RenderView(ViewContext viewContext)
        {
            ViewContext = viewContext;
            InitHelpers();
            ID = Guid.NewGuid().ToString();

            var response = new HttpResponse(viewContext.Writer);
            var context = new HttpContext(HttpContext.Current.Request, response) { User = viewContext.HttpContext.User };
            foreach (DictionaryEntry contextItem in viewContext.HttpContext.Items)
            {
                context.Items[contextItem.Key] = contextItem.Value;
            }

            ProcessRequest(context);
        }
    }
}
