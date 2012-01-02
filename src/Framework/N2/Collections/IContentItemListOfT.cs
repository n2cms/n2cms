namespace N2.Collections
{
	public interface IContentItemList<T> : IContentList<T>, IZonedList<T>
		where T : ContentItem
	{
		int EnclosingItemID { get; }
	}
}
