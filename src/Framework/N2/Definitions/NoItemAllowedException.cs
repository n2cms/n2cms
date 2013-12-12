namespace N2.Definitions
{
    /// <summary>This exception is thrown when trying to adding an item to a parent that doesn't support any types of child items.</summary>
    public class NoItemAllowedException : N2Exception
    {
        public NoItemAllowedException(ContentItem parentItem)
            : base("No item is allowed below " + parentItem.ID)
        {
            this.parentItem = parentItem;
        }

        private ContentItem parentItem;

        public ContentItem ParentItem
        {
            get { return parentItem; }
        }
    }
}
