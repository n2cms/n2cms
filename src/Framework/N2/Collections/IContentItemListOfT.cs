using N2.Persistence;
using System.Collections.Generic;
namespace N2.Collections
{
    public interface IContentItemList<T> : IContentList<T>, IZonedList<T>
        where T : ContentItem
    {
		int EnclosingItemID { get; }
    }
}
