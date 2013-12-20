using System;
using System.Web.Mvc;
using N2.Web.UI;

namespace N2.Web.Mvc
{
    /// <summary>
    /// A ViewMasterPage implementation that allows N2 Display helpers to be used.
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    [Obsolete("Use System.Web.Mvc.ViewMasterPage<>")]
    public class N2ViewMasterPage<TItem> : ViewMasterPage<TItem>, IItemContainer<TItem>
        where TItem : ContentItem
    {
        public TItem CurrentItem
        {
            get { return Model; }
        }

        ContentItem IItemContainer.CurrentItem
        {
            get { return CurrentItem; }
        }
    }
}
