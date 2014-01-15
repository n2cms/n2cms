using System;

namespace N2.Web.UI
{
    /// <summary>
    /// This class name has been deprecated. A user control that can be dynamically created, bound to non-page items and added to a page.
    /// </summary>
    /// <typeparam name="TPage">The type of page item this user control will have to deal with.</typeparam>
    /// <typeparam name="TItem">The type of non-page (data) item this user control will be bound to.</typeparam>
    [Obsolete("Please use N2.Web.UI.ContentUserControl instead.")]
    public abstract class UserControl<TPage, TItem> : ContentUserControl<TPage, TItem>, IItemContainer, IContentTemplate
        where TPage : N2.ContentItem
        where TItem : N2.ContentItem
    {
    }
}
