using N2.Persistence;
using System.Collections.Generic;
namespace N2.Collections
{
    public interface IContentItemList<T> : IContentList<T>, IZonedList<T>
        where T : ContentItem
    {
        IEnumerable<T> Find(IParameter parameters);
        IEnumerable<IDictionary<string, object>> Select(IParameter parameters, params string[] properties);
        int FindCount(IParameter parameters);
    }
}
