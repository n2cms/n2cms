using System;

namespace N2.Persistence.Search
{
    public class IndexStatistics
    {
        public int TotalDocuments { get; set; }
    }

	/// <summary>
	/// Indexes content items to be searched.
	/// </summary>
	public interface IIndexer
	{
        /// <summary>Gets index statistics.</summary>
        /// <returns>The current statistics on the index.</returns>
        IndexStatistics GetStatistics();

		/// <summary>Clears the index.</summary>
		void Clear();

		/// <summary>Optimizes the index.</summary>
		void Optimize();

		/// <summary>Delets an item from the index and any descendants.</summary>
		/// <param name="itemID">The id of the item to delete.</param>
		void Delete(int itemID);

		/// <summary>Updates the index with the given item.</summary>
		/// <param name="item">The item containing content to be indexed.</param>
		void Update(ContentItem item);

		/// <summary>Unlocks the index.</summary>
		void Unlock();
    }
}
