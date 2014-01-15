namespace N2.Persistence
{
    /// <summary>
    /// Used by the versioning manager to ask the master version of 
    /// a content item to update it's data to another version.
    /// </summary>
    /// <typeparam name="T">Probably ContentItem.</typeparam>
    public interface IUpdatable<T>
    {
        void UpdateFrom(T item);
    }
}
