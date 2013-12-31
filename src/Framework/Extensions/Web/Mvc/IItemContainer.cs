using N2.Web.UI;

namespace N2.Web.Mvc
{
    public interface IItemContainer<TItem> : IItemContainer
        where TItem : ContentItem
    {
        new TItem CurrentItem { get; }
    }
}
