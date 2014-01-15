namespace N2.Edit
{
    /// <summary>
    /// Used to resolve document base when applied to a content item. The document base 
    /// is used to resolve the location of non-rooted links and sources in the editor
    /// html source.
    /// </summary>
    public interface IDocumentBaseSource
    {
        /// <summary>An url with a trailing slash used to prepend relative managementUrls.</summary>
        string BaseUrl { get; }
    }
}
