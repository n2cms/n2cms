using System;
using System.Collections.Generic;

namespace N2.Persistence.Search
{
    public class IndexStatistics
    {
        public int TotalDocuments { get; set; }
    }

	public class IndexField
	{
		public string Name { get; set; }
		public string Value { get; set; }
		public bool Analyzed { get; set; }
		public bool Stored { get; set; }
	}

	public class IndexDocument
	{
		public IndexDocument()
		{
			Values = new IndexField[0];
			Tokens = new Dictionary<string, object>();
		}

		public int ID { get; set; }
		public IEnumerable<IndexField> Values { get; set; }
		public IDictionary<string, object> Tokens { get; set; }
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

		/// <summary>Tells whether the item is suitable for indexing.</summary>
		/// <param name="item">The item to check.</param>
		/// <returns>True if the item would be indexed.</returns>
		bool IsIndexable(ContentItem item);

		/// <summary>Delets an item from the index and any descendants.</summary>
		/// <param name="itemID">The id of the item to delete.</param>
		void Delete(int itemID);

		/// <summary>Updates the index with the given item.</summary>
		/// <param name="item">The item containing content to be indexed.</param>
		void Update(ContentItem item);

		/// <summary>Creates a document that can be indexed.</summary>
		/// <param name="item">The item containing values to be indexed.</param>
		/// <returns>A document that can be passed to the Update method to be indexed.</returns>
		IndexDocument CreateDocument(ContentItem item);
		
		/// <summary>Updates the indexing using the given document.</summary>
		/// <param name="document"></param>
		void Update(IndexDocument document);

		/// <summary>Unlocks the index.</summary>
		void Unlock();
    }
}
