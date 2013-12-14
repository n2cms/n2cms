namespace N2.Persistence
{
    /// <summary>
    /// Event argument used by <see cref="IItemNotifier"/> to notify about changes during interception.
    /// </summary>
    public class NotifiableItemEventArgs : ItemEventArgs
    {
        public NotifiableItemEventArgs(ContentItem item)
            : base(item)
        {
        }

        /// <summary>Set to true when modifying items using this event argument.</summary>
        public bool WasModified { get; set; }
    }
}
