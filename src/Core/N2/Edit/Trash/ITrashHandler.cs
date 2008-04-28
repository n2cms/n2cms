using System;
namespace N2.Edit.Trash
{
	public interface ITrashHandler
	{
		ITrashCan TrashContainer { get; }

		bool CanThrow(N2.ContentItem affectedItem);
		bool IsInTrash(ContentItem item);

		void ExpireTrashedItem(N2.ContentItem item);
		void Restore(N2.ContentItem item);
		void RestoreValues(N2.ContentItem item);
		void Throw(N2.ContentItem item);
	}
}
