using System;
using System.Web.Mvc;
using N2.Web.UI;
using N2.Web.Mvc.Html;

namespace N2.Web.Mvc
{
    /// <summary>
    /// A ViewUserControl implementation that allows N2 Display helpers to be used
    /// 
    /// This class only restricts the model by enforcing that it implements the lightweight <see cref="IItemContainer{TItem}" /> interface.
    /// This way a Model needn't be an N2 ContentItem
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TItem"></typeparam>
    public class ContentViewUserControl<TModel, TItem> : ViewUserControl<TModel>, IItemContainer
        where TModel : class
        where TItem : ContentItem
    {
        #region IItemContainer Members

        ContentItem IItemContainer.CurrentItem
        {
            get { return Content; }
        }

        #endregion

        public TItem content;
        public TItem Content
        {
            get { return content ?? (content = ViewContext.CurrentItem<TItem>()); }
            set { content = value; }
        }

        HtmlHelper<TItem> contentHtml;
        public HtmlHelper<TItem> ContentHtml
        {
            get { return contentHtml ?? (contentHtml = ViewContext.CreateContentItemHelper<TItem>()); }
            set { contentHtml = value; }
        }
    }
}
