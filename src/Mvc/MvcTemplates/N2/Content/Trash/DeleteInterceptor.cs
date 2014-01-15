using N2.Engine;
using N2.Persistence;
using N2.Plugin;

namespace N2.Edit.Trash
{
    /// <summary>
    /// Intercepts delete operations.
    /// </summary>
    [Service]
    public class DeleteInterceptor : IAutoStart
    {
        private readonly IPersister persister;
        private readonly ITrashHandler trashHandler;

        public DeleteInterceptor(IPersister persister, ITrashHandler trashHandler)
        {
            this.persister = persister;
            this.trashHandler = trashHandler;
        }

        public void Start()
        {
            persister.ItemDeleting += OnItemDeleting;
            persister.ItemMoving += OnItemMoved;
            persister.ItemCopied += OnItemCopied;
        }

        public void Stop()
        {
            persister.ItemDeleting -= OnItemDeleting;
            persister.ItemMoving -= OnItemMoved;
            persister.ItemCopied -= OnItemCopied;
        }

        private void OnItemCopied(object sender, DestinationEventArgs e)
        {
            if(LeavingTrash(e))
            {
                trashHandler.RestoreValues(e.AffectedItem);
            }
            else if (trashHandler.IsInTrash(e.Destination))
            {
                trashHandler.ExpireTrashedItem(e.AffectedItem);
            }
        }

        private void OnItemMoved(object sender, CancellableDestinationEventArgs e)
        {
            if (LeavingTrash(e))
            {
                trashHandler.RestoreValues(e.AffectedItem);
            }
            else if (trashHandler.IsInTrash(e.Destination))
            {
                trashHandler.ExpireTrashedItem(e.AffectedItem);
            }
        }

        private void OnItemDeleting(object sender, CancellableItemEventArgs e)
        {
            if (trashHandler.CanThrow(e.AffectedItem))
            {
                e.FinalAction = trashHandler.Throw;
            }
        }

        private bool LeavingTrash(DestinationEventArgs e)
        {
            return e.AffectedItem["DeletedDate"] != null && !trashHandler.IsInTrash(e.Destination);
        }
    }
}
