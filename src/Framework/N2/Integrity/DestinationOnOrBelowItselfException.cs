namespace N2.Integrity
{
    /// <summary>
    /// Exception thrown when an attempt to move an item onto or below itself is made.
    /// </summary>
    public class DestinationOnOrBelowItselfException : N2Exception
    {
        public DestinationOnOrBelowItselfException(ContentItem source, ContentItem destination)
            : base("Cannot move item to a destination onto or below itself.")
        {
            this.sourceItem = source;
            this.destinationItem = destination;
        }

        private ContentItem sourceItem;
        private ContentItem destinationItem;

        /// <summary>Gets the source item that is causing the conflict.</summary>
        public ContentItem SourceItem
        {
            get { return sourceItem; }
        }

        /// <summary>Gets the parent item already containing an item with the same name.</summary>
        public ContentItem DestinationItem
        {
            get { return destinationItem; }
        }

    }
}
