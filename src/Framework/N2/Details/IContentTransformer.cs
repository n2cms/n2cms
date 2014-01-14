namespace N2.Details
{
    /// <summary>
    /// Applies modifications to a content item before it enters a state.
    /// </summary>
    public interface IContentTransformer
    {
        /// <summary>Apply modifications before transitioning item to this state.</summary>
        /// <remarks>Only New is currently supported.</remarks>
        ContentState ChangingTo { get; }

        /// <summary>Applies modifications to the given item.</summary>
        /// <param name="item">The item to modify.</param>
        bool Transform(ContentItem item);
    }
}
