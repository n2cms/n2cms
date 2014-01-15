using System;

namespace N2.Web.Mvc
{
    [Obsolete("Use ContentViewUserControl<TModel, TItem>")]
    public class N2ModelViewUserControl<TModel, TItem> : ContentViewUserControl<TModel, TItem>, IItemContainer<TItem>
        where TModel : class, IItemContainer<TItem>
        where TItem : ContentItem
    {
    }
}
