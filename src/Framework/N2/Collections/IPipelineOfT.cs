using System.Collections.Generic;

namespace N2.Collections
{
    /// <summary>
    /// Filters items.
    /// </summary>
    public interface IPipeline<T>
    {
        /// <summary>Filter the given items returning the filtered enumeration.</summary>
        /// <param name="input">The input enumeration to filter.</param>
        /// <returns>A filtered enumeration of items.</returns>
        IEnumerable<T> Pipe(IEnumerable<T> input);
    }
}
