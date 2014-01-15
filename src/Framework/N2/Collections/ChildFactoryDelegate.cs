using System.Collections.Generic;

namespace N2.Collections
{
    /// <summary>A method that gets the children of the given item.</summary>
    /// <param name="item">The item whose children to get.</param>
    /// <returns>An enumeration of the children of the item.</returns>
    public delegate IEnumerable<ContentItem> ChildFactoryDelegate(ContentItem item);
}
