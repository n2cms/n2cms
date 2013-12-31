using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Persistence.Search
{
    public class IndexStatistics
    {
        public int TotalDocuments { get; set; }
    }

    public interface IIndexer
    {
        /// <summary>Updates the indexing using the given document.</summary>
        /// <param name="document"></param>
        void Update(IndexableDocument document);

        /// <summary>Delets an item from the index and any descendants.</summary>
        /// <param name="itemID">The id of the item to delete.</param>
        void Delete(int itemID);

        /// <summary>Gets index statistics.</summary>
        /// <returns>The current statistics on the index.</returns>
        IndexStatistics GetStatistics();

        /// <summary>Clears the index.</summary>
        void Clear();

        /// <summary>Optimizes the index.</summary>
        void Optimize();

        /// <summary>Unlocks the index.</summary>
        void Unlock();
    }
}
