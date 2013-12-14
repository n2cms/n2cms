using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using N2.Engine;
using N2.Engine.Globalization;
using N2.Web;
using System;

namespace N2.Persistence.Search
{
    /// <summary>
    /// Wraps the usage of lucene to index content items.
    /// </summary>
    [Service(typeof(IContentIndexer))]
    public class ContentIndexer : IContentIndexer
    {
        private TextExtractor extractor;
        private IIndexer indexer;

        public ContentIndexer(IIndexer indexer, TextExtractor extractor)
        {
            this.extractor = extractor;
            this.indexer = indexer;
        }

        /// <summary>Clears the index.</summary>
        public virtual void Clear()
        {
            indexer.Clear();
        }

        /// <summary>Unlocks the index.</summary>
        public virtual void Unlock()
        {
            indexer.Unlock();
        }

        /// <summary>Optimizes the index.</summary>
        public virtual void Optimize()
        {
            indexer.Optimize();
        }

        /// <summary>Updates the index with the given item.</summary>
        /// <param name="item">The item containing content to be indexed.</param>
        public virtual void Update(ContentItem item)
        {
            if (!IsIndexable(item))
                return;

            if (!item.IsPage)
                Update(N2.Content.Traverse.ClosestPage(item));

            Update(extractor.CreateDocument(item));
        }

        public virtual bool IsIndexable(ContentItem item)
        {
            if (item == null || item.ID == 0)
                return false;
            return extractor.IsIndexable(item);
        }

        /// <summary>Delets an item from the index and any descendants.</summary>
        /// <param name="itemID">The id of the item to delete.</param>
        public virtual void Delete(int itemID)
        {
            indexer.Delete(itemID);
        }

        public IndexableDocument CreateDocument(ContentItem item)
        {
            return extractor.CreateDocument(item);
        }

        public void Update(IndexableDocument document)
        {
            indexer.Update(document);
        }

        public IndexStatistics GetStatistics()
        {
            return indexer.GetStatistics();
        }
    }
}
