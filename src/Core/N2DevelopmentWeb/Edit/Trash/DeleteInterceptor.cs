using System;
using System.Collections.Generic;
using System.Text;
using Castle.Core;
using N2.Persistence;

namespace N2.Trashcan
{
	/// <summary>
	/// Intercepts delete operations.
	/// </summary>
	public class DeleteInterceptor : IStartable
	{
		private readonly IPersister persister;
		private readonly TrashHandler trashHandler;

		public DeleteInterceptor(IPersister persister, TrashHandler trashHandler)
		{
			this.persister = persister;
			this.trashHandler = trashHandler;
		}

		public void Start()
		{
			persister.ItemDeleting += ItemDeletingEventHandler;
			persister.ItemMoved += ItemMovedEventHandler;
			persister.ItemCopied += ItemCopiedEventHandler;
		}

		public void Stop()
		{
			persister.ItemDeleting -= ItemDeletingEventHandler;
			persister.ItemMoved -= ItemMovedEventHandler;
			persister.ItemCopied -= ItemCopiedEventHandler;
		}

		private void ItemCopiedEventHandler(object sender, DestinationEventArgs e)
		{
			if(LeavingTrash(e))
			{
				trashHandler.RestoreValues(e.AffectedItem);
			}
			else if (IsInTrash(e.Destination))
			{
				trashHandler.ExpireTrashedItem(e.AffectedItem);
			}
		}

		private void ItemMovedEventHandler(object sender, DestinationEventArgs e)
		{
			if (LeavingTrash(e))
			{
				trashHandler.RestoreValues(e.AffectedItem);
			}
			else if (IsInTrash(e.Destination))
			{
				trashHandler.ExpireTrashedItem(e.AffectedItem);
			}
		}

		private void ItemDeletingEventHandler(object sender, CancellableItemEventArgs e)
		{
			if (trashHandler.CanThrow(e.AffectedItem))
			{
				e.Cancel = true;
				trashHandler.Throw(e.AffectedItem);
			}
		}

		private bool LeavingTrash(DestinationEventArgs e)
		{
			return e.AffectedItem["DeletedDate"] != null && !IsInTrash(e.Destination);
		}

		private bool IsInTrash(ContentItem item)
		{
			return Find.IsDescendantOrSelf(item, trashHandler.TrashContainer);
		}
	}
}
