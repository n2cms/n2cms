using System;

namespace N2.Edit.Trash
{
    public class PurgingStatus
    {
        public int Deleted { get; set; }
        public int Remaining { get; set; }
        
        public int Total
        {
            get { return Deleted + Remaining; }
        }
    }

    /// <summary>
    /// Service interface implemented by the trash handler implementation.
    /// </summary>
    public interface ITrashHandler
    {
        /// <summary>The container of thrown items.</summary>
        ITrashCan TrashContainer { get; }

        /// <summary>Checks if the trash is enabled, the item is not already thrown and for the NotThrowable attribute.</summary>
        /// <param name="affectedItem">The item to check.</param>
        /// <returns>True if the item may be thrown.</returns>
        bool CanThrow(N2.ContentItem affectedItem);

        /// <summary>Determines wether an item has been thrown away.</summary>
        /// <param name="item">The item to check.</param>
        /// <returns>True if the item is in the scraps.</returns>
        bool IsInTrash(ContentItem item);

        /// <summary>Expires an item that has been thrown so that it's not accessible to external users.</summary>
        /// <param name="item">The item to restore.</param>
        void ExpireTrashedItem(N2.ContentItem item);

        /// <summary>Restores an item to the original location.</summary>
        /// <param name="item">The item to restore.</param>
        void Restore(N2.ContentItem item);

        /// <summary>Removes expiry date and metadata set during throwing.</summary>
        /// <param name="item">The item to reset.</param>
        void RestoreValues(N2.ContentItem item);

        /// <summary>Throws an item in a way that it later may be restored to it's original location at a later stage.</summary>
        /// <param name="item">The item to throw.</param>
        void Throw(N2.ContentItem item);

        /// <summary>Occurs before an item is thrown.</summary>
        event EventHandler<CancellableItemEventArgs> ItemThrowing;
        /// <summary>Occurs after an item has been thrown.</summary>
        event EventHandler<ItemEventArgs> ItemThrowed;

        /// <summary>Delete items lying in trash for longer than the specified interval.</summary>
        void PurgeOldItems();

        void PurgeAll(Action<PurgingStatus> onProgress = null);

        void Purge(ContentItem itemToPurge, Action<PurgingStatus> onProgress = null);

		void HandleMoved(ContentItem from);
	}
}
