using N2.Collections;

namespace N2.Edit.Trash
{
    /// <summary>
    /// Container of scrapped items.
    /// </summary>
	public interface ITrashCan
	{
        /// <summary>Whether the trash functionality is enabled.</summary>
		bool Enabled { get; }

		/// <summary>Number of days after which deleted items should be purged from trash.</summary>
		TrashPurgeInterval PurgeInterval { get; }

        /// <summary>Currently thrown items.</summary>
		IContentItemList<ContentItem> Children { get; }
	}
}
