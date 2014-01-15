namespace N2.Web
{
    /// <summary>
    /// When implemented by an attribute placed on a content item this 
    /// interface can resolve the path to a template.
    /// </summary>
    public interface IPathFinder
    {
        PathData GetPath(ContentItem item, string remainingUrl);
    }
}
