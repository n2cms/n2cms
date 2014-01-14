using System;
using System.Web.Mvc;
using N2.Web.UI;

namespace N2.Web.Mvc
{
    /// <summary>
    /// A ViewUserControl implementation that allows N2 Display helpers to be used.
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    [Obsolete("Prefer System.Web.Mvc.ViewUserControl<>")]
    public class N2ViewUserControl<TItem> : ViewUserControl<TItem>, IItemContainer<TItem>
        where TItem : ContentItem
    {
        #region IItemContainer<TItem> Members

        public TItem CurrentItem
        {
            get { return Model; }
        }

        ContentItem IItemContainer.CurrentItem
        {
            get { return CurrentItem; }
        }

        #endregion

        public ContentItem CurrentPage
        {
            get
            {
                ContentItem page = CurrentItem;

                while(page != null && !page.IsPage)
                {
                    page = page.Parent;
                }

                return page;
            }
        }
    }
}
