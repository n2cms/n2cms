namespace N2
{
    /// <summary>
    /// Event argument containing item and destination item.
    /// </summary>
    public class CancellableDestinationEventArgs : DestinationEventArgs
    {
        private bool cancel;

        public CancellableDestinationEventArgs(ContentItem item, ContentItem destination)
            : base(item, destination)
        {
        }

        /// <summary>Gets or sets whether the event with this argument should be cancelled.</summary>
        public bool Cancel
        {
            get { return cancel; }
            set { cancel = value; }
        }
    }
}