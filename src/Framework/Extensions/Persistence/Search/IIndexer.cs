using System;

namespace N2.Persistence.Search
{
	/// <summary>
	/// Indexes content items to be searched.
	/// </summary>
	public interface IIndexer
	{
		/// <summary>Clears the index.</summary>
		void Clear();

		/// <summary>Optimizes the index.</summary>
		void Optimize();

		/// <summary>Delets an item from the index and any descendants.</summary>
		/// <param name="itemID">The id of the item to delete.</param>
		void Delete(int itemID);

		/// <summary>Updates the index with the given item.</summary>
		/// <param name="item">The item containing content to be indexed.</param>
		void Update(N2.ContentItem item);
	}
}
