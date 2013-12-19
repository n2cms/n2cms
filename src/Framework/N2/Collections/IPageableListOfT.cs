using System.Collections.Generic;
using System.Linq;

namespace N2.Collections
{
    /// <summary>
    /// Retrieves a subset of the list.
    /// </summary>
    /// <typeparam name="T">The type of item in the list.</typeparam>
    public interface IPageableList<T>
    {
        /// <summary>Gets a subset of the list without causing the complete list to be loaded.</summary>
        /// <param name="skip">The number of items to skip.</param>
        /// <param name="take">The number of items to take.</param>
        /// <returns>A list of items within the given range.</returns>
        IQueryable<T> FindRange(int skip, int take);
    }
}
