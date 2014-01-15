using System;
using N2.Persistence.Search;

namespace N2.Definitions
{
    /// <summary>
    /// Marks an item that is available for syndication.
    /// </summary>
    public interface ISyndicatable
    {
        /// <summary>The title of the syndicated item.</summary>
        string Title { get; }

        /// <summary>When the item was published.</summary>
        DateTime? Published { get; }

        /// <summary>The address to the item.</summary>
        string Url { get; }

        /// <summary>A content summary.</summary>
        string Summary { get; }

        /// <summary>Whether this particular item is to be syndicated.</summary>
        bool Syndicate { get; set; }
    }
}
