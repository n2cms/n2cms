using System.Collections.Generic;

namespace N2.Edit
{
    /// <summary>
    /// Provides virtual nodes for the management interface.
    /// </summary>
    public interface INodeProvider
    {
        /// <summary>Gets the content item associated with a path.</summary>
        /// <param name="path">The path corresponding to a node.</param>
        /// <returns>The item or null if not a matching path.</returns>
        ContentItem Get(string path);

        /// <summary>Gets children associated with a parent path.</summary>
        /// <param name="path">The parent path of the provided nodes.</param>
        /// <returns>A list of nodes below the given path.</returns>
        IEnumerable<ContentItem> GetChildren(string path);
    }
}
