using N2.Edit.Trash;

namespace N2.Extensions
{
	public static class TrashExtensions
	{
		/// <summary>
		/// Checks if the item contained within a Trash Can.
		/// </summary>
		/// <param name="item">The item to check.</param>
		/// <returns>True if the item is in the trash can.</returns>
		public static bool IsRecycled(this ContentItem item)
		{
			foreach(ContentItem ancestor in Find.EnumerateParents(item))
			{
				if (ancestor is ITrashCan)
					return true;
			}
			return false;
		}
	}
}
