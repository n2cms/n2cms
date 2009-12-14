using System;
using System.Collections;
using System.Web;
using System.Web.Mvc;
using N2.Web.UI;
using N2.Web.Mvc.Html;

namespace N2.Web.Mvc
{
    [Obsolete("Name changed to ContentViewPage")]
    public class N2ModelViewPage<TModel, TItem> : ContentViewPage<TModel, TItem>, IItemContainer<TItem>
        where TModel : class
        where TItem : ContentItem
    {
        #region IItemContainer<TItem> Members

        public TItem CurrentItem
        {
            get { return Content; }
            set { Content = value; }
        }

        #endregion

        public override void RenderView(ViewContext viewContext)
        {
            ViewContext = viewContext;
            InitHelpers();
            ID = Guid.NewGuid().ToString();

            var response = new HttpResponse(viewContext.HttpContext.Response.Output);
            var context = new HttpContext(HttpContext.Current.Request, response) { User = viewContext.HttpContext.User };
            foreach (DictionaryEntry contextItem in viewContext.HttpContext.Items)
            {
                context.Items[contextItem.Key] = contextItem.Value;
            }

            ProcessRequest(context);
        }
    }
}