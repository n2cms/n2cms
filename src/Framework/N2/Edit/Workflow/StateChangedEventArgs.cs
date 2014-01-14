namespace N2.Edit.Workflow
{
    /// <summary>
    /// Converys information about state changes
    /// </summary>
    public class StateChangedEventArgs : ItemEventArgs
    {
        public StateChangedEventArgs(ContentItem item, ContentState previousState)
            : base(item)
        {
            PreviousState = previousState;
        }

        /// <summary>The state the item had before the change.</summary>
        public ContentState PreviousState { get; private set; }
    }
}
