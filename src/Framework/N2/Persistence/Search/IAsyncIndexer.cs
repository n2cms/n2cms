using System;
namespace N2.Persistence.Search
{
    public class IndexStatus
    {
        public string CurrentWork { get; set; }

        public int WorkerCount { get; set; }

        public int ErrorQueueCount { get; set; }
    }

    /// <summary>
    /// Indexes on a worker thread.
    /// </summary>
    public interface IAsyncIndexer
    {
        /// <summary>Gets a snapshot of the current index status.</summary>
        /// <returns></returns>
        IndexStatus GetCurrentStatus();

        /// <summary>Deletes an item.</summary>
        /// <param name="itemID"></param>
        void Delete(int itemID);

        /// <summary>Creates or updates an item index.</summary>
        /// <param name="itemID"></param>
        /// <param name="affectsChildren"></param>
        void Reindex(int itemID, bool affectsChildren);

        /// <summary>Reindexes an item and it's descendants.</summary>
        /// <param name="root"></param>
		/// <param name="clearBeforeReindex"></param>
        void ReindexDescendants(N2.ContentItem root, bool clearBeforeReindex);
    }
}
