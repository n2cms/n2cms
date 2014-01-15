using System.Collections.Generic;
using System.Linq;

namespace N2.Collections
{
    /// <summary>
    /// A list that supports sub-selecting items that are placeable.
    /// </summary>
    /// <typeparam name="T">The type of item in the list.</typeparam>
    public interface IZonedList<T> where T : class, IPlaceable
    {
        /// <summary>Retrieves visible and published pages where the zone name is null.</summary>
        /// <returns>A list of items that are considered to be pages.</returns>
        /// <remarks>This method does not perform security checks.</remarks>
        IEnumerable<T> FindNavigatablePages();

        /// <summary>Retrieves pages where the zone name is null.</summary>
        /// <returns>A list of items that are considered to be pages.</returns>
        IEnumerable<T> FindPages();

        /// <summary>Retrieves items where the zone name is not null.</summary>
        /// <returns>A list of items that are considered to be parts.</returns>
        IEnumerable<T> FindParts();

        /// <summary>Retrieves items a zone with a particular name.</summary>
        /// <param name="zoneName">The name of the zone.</param>
        /// <returns>A list of items in the zone.</returns>
        IEnumerable<T> FindParts(string zoneName);

        /// <summary>Retrieves names of zones in this list.</summary>
        /// <returns>A distinct set of zone names.</returns>
        IEnumerable<string> FindZoneNames();
    }
}
