using System;
using System.Collections.Generic;

namespace N2.Collections
{
    /// <summary>
    /// Classes implementing this interface provides funtionality to navigate
    /// between graphs of items.
    /// </summary>
    /// <typeparam name="T">The type of node to navigate.</typeparam>
    [Obsolete("Use hierarchy builders and hierarchy nodes")]
    public interface IHierarchyNavigator<T>
        where T : class
    {
        /// <summary>Gets the parent node navigator.</summary>
        IHierarchyNavigator<T> Parent { get; }

        /// <summary>Gets all child node navigators.</summary>
        IEnumerable<IHierarchyNavigator<T>> Children { get; }

        /// <summary>Gets the current node.</summary>
        T Current { get; }

        /// <summary>Gets wether the current item has any child nodes.</summary>
        bool HasChildren { get; }
    }
}
