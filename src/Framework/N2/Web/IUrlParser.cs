using System;

namespace N2.Web
{
    /// <summary>
    /// Classes implementing this interface creates unique managementUrls for items and
    /// can from such an url find the corresponding item.
    /// </summary>
    public interface IUrlParser
    {
        /// <summary>Is invoked when the url parser didn't find </summary>
        event EventHandler<PageNotFoundEventArgs> PageNotFound;

        /// <summary>Is invoked while generating the url of an item.</summary>
        event EventHandler<UrlEventArgs> BuildingUrl;

        /// <summary>Is invoked after generating the url of an item.</summary>
        event EventHandler<UrlEventArgs> BuiltUrl;

        /// <summary>Gets the current start page.</summary>
        ContentItem StartPage { get; }

        /// <summary>Parses the current url to retrieve the current page.</summary>
        ContentItem CurrentPage { get; }

        /// <summary>Calculates an item url by walking it's parent path.</summary>
        /// <param name="item">The item whose url to compute.</param>
        /// <returns>A friendly url to the supplied item.</returns>
        Url BuildUrl(ContentItem item);

        /// <summary>Checks if an item is start or root page</summary>
        /// <param name="item">The item to check</param>
        /// <returns>True if the item is a start page or a root page</returns>
        bool IsRootOrStartPage(ContentItem item);

        /// <summary>Finds the path associated with an url.</summary>
        /// <param name="url">The url to the template to locate.</param>
        /// <param name="startNode">The node to start finding path from if none supplied will start from StartNode</param>
        /// <param name="remainingPath">The remaining path to search</param>
        /// <returns>A PathData object. If no template was found the object will have empty properties.</returns>
        PathData FindPath(Url url, ContentItem startNode = null, string remainingPath = null);

        [Obsolete("Use FindPath")]
        /// <summary>Finds the content item and the template associated with an url.</summary>
        /// <param name="url">The url to the template to locate.</param>
        /// <param name="startNode">The node to start finding path from if none supplied will start from StartNode</param>
        /// <param name="remainingPath">The remaining path to search</param>
        /// <returns>A PathData object. If no template was found the object will have empty properties.</returns>
        PathData ResolvePath(Url url, ContentItem startNode = null, string remainingPath = null);

        /// <summary>Finds an item by traversing names from the starting point root.</summary>
        /// <param name="url">The url that should be traversed.</param>
        /// <returns>The content item matching the supplied url.</returns>
        ContentItem Parse(string url);

        /// <summary>Removes a trailing Default.aspx from an URL.</summary>
        /// <param name="path">A URL path without query strings from which to remove any trailing Default.aspx.</param>
        /// <returns>The same path or one stripped of the remaining default document segment.</returns>
        string StripDefaultDocument(string path);
    }
}
