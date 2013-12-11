using System;
using System.Collections.Generic;

namespace N2.Persistence.Search
{
    /// <summary>
    /// Indexes content items to be searched.
    /// </summary>
    public interface IContentIndexer : IIndexer
    {
        /// <summary>Tells whether the item is suitable for indexing.</summary>
        /// <param name="item">The item to check.</param>
        /// <returns>True if the item would be indexed.</returns>
        bool IsIndexable(ContentItem item);

        /// <summary>Updates the index with the given item.</summary>
        /// <param name="item">The item containing content to be indexed.</param>
        void Update(ContentItem item);

        /// <summary>Creates a document that can be indexed.</summary>
        /// <param name="item">The item containing values to be indexed.</param>
        /// <returns>A document that can be passed to the Update method to be indexed.</returns>
        IndexableDocument CreateDocument(ContentItem item);
    }
}
