using System;
namespace N2.Persistence.Finder
{
    /// <summary>
    /// Classes implementing this interface are responsible creating a query 
    /// builder.
    /// </summary>
    public interface IItemFinder
    {
        /// <summary>
        /// Starts the building of a query.
        /// </summary>
        IQueryBuilder Where { get; }

        /// <summary>
        /// Allows selection of all items.
        /// </summary>
        [Obsolete("Avoid! This will retrieve all items from the database")]
        IQueryEnding All { get; }
    }
}
