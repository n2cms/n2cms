using System;
namespace N2.Edit
{
    public interface ITreeSorter
    {
        void MoveUp(ContentItem item);
        void MoveDown(ContentItem item);
        void MoveTo(ContentItem item, ContentItem parent);
        void MoveTo(ContentItem item, ContentItem parent, int index);
        void MoveTo(ContentItem item, int index);
        void MoveTo(ContentItem item, NodePosition position, ContentItem relativeTo);

		event EventHandler<DestinationEventArgs> ItemMoved;
    }
}
