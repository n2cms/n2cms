using System;

namespace N2
{
    /// <summary>
    /// Event argument containing item and destination item.
    /// </summary>
    public class CancellableDestinationEventArgs : DestinationEventArgs
    {
        private bool cancel;
        private Func<ContentItem, ContentItem, ContentItem> finalAction;

        public CancellableDestinationEventArgs(ContentItem item, ContentItem destination, Func<ContentItem, ContentItem, ContentItem> finalAction)
            : base(item, destination)
        {
            this.finalAction = finalAction;
        }

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

        /// <summary>The action to execute unless the event is cancelled. This action can be exchanged by observers to alter the default behaviour.</summary>
        public Func<ContentItem, ContentItem, ContentItem> FinalAction
        {
            get { return finalAction; }
            set { finalAction = value; }
        }
    }
}
