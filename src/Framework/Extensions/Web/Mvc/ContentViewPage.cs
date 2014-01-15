using System.Web.Mvc;
using N2.Web.Mvc.Html;
using N2.Web.UI;

namespace N2.Web.Mvc
{

    /// <summary>
    /// A ViewPage implementation that allows N2 Display helpers to be used
    /// 
    /// This class only restricts the model by enforcing that it implements the lightweight <see cref="IItemContainer{TItem}" /> interface.
    /// This way a Model needn't be an N2 ContentItem
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TItem"></typeparam>
    public class ContentViewPage<TModel, TItem> : ViewPage<TModel>, IItemContainer
        where TModel : class
        where TItem : ContentItem
    {
        #region IItemContainer Members

        ContentItem IItemContainer.CurrentItem
        {
            get { return CurrentItem; }
        }

        #endregion

        public HtmlHelper<TItem> ContentHtml { get; set; }
        public TItem CurrentItem { get; set; }

        public override void InitHelpers()
        {
            base.InitHelpers();

            CurrentItem = ViewContext.RouteData.CurrentItem() as TItem;
            ContentHtml = ViewContext.CreateContentItemHelper<TItem>();
        }
    }
}
