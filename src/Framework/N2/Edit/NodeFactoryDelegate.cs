namespace N2.Edit
{
    /// <summary>Represents a method that creates a content item based on a path.</summary>
    /// <param name="path">The relative path to resolve</param>
    /// <returns>The content item at the given path or null.</returns>
    public delegate ContentItem NodeFactoryDelegate(string relativePath);
}
