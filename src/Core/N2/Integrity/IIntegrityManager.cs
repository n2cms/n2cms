using System;
using System.Web;

namespace N2.Integrity
{
	/// <summary>
	/// Classes implementing this interface are responsible of the integrity 
	/// between content items in a N2 cms systems. This probably involves 
	/// checking which items may be placed below which and ensuring that no 
	/// items are placed below themselves.
	/// </summary>
	public interface IIntegrityManager
	{
		/// <summary>Check wether a copy operation is allowed.</summary>
		/// <param name="source">The item to copy.</param>
		/// <param name="destination">The destination for the copied item.</param>
		/// <returns>True if the item can becopied.</returns>
		bool CanCopy(ContentItem source, ContentItem destination);

		/// <summary>Checks wether an item can be deleted</summary>
		/// <param name="item">The item to delete.</param>
		/// <returns>True if the item can be deleted.</returns>
		bool CanDelete(ContentItem item);

		/// <summary>Check wether a move operation is allowed.</summary>
		/// <param name="source">The item to move.</param>
		/// <param name="destination">The parent item where the item will be moved to.</param>
		/// <returns>True if the item can be moved.</returns>
		bool CanMove(ContentItem source, ContentItem destination);

		/// <summary>Checks wether an item can be saved.</summary>
		/// <param name="item">The item to save.</param>
		/// <returns>True if the item can be saved.</returns>
		bool CanSave(ContentItem item);

		/// <summary>Checks wether an item's name is locally unique, i.e. no other sibling has the same name.</summary>
		/// <param name="name">The name we're proposing for the item.</param>
		/// <param name="item">The item whose siblings to check.</param>
		/// <returns>True if the item would get a unique name.</returns>
		bool IsLocallyUnique(string name, ContentItem item);

		N2Exception GetMoveException(ContentItem source, ContentItem destination);
		N2Exception GetCopyException(ContentItem source, ContentItem destination);
		N2Exception GetDeleteException(ContentItem item);
		N2Exception GetSaveException(ContentItem item);
	}
}
